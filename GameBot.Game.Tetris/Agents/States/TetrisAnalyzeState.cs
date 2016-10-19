using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using GameBot.Game.Tetris.Extraction;

namespace GameBot.Game.Tetris.Agents.States
{
    public class TetrisAnalyzeState : ITetrisAgentState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly TetrisAgent _agent;

        private Tetromino? _currentTetromino;

        private readonly IList<ProbabilisticResult<Piece>> _extractedPieceSamples;
        private readonly IList<ProbabilisticResult<Tetromino>> _extractedNextPieceSamples;

        private Piece _extractedPiece;
        private TimeSpan _extractedPieceTimestamp;
        private Tetromino? _extractedNextPiece;
        private TimeSpan? _beginTime;
        
        // we can extract the next piece only then, when we already have found the current piece
        private bool CanExtractNextPiece => _extractedPiece != null || _extractedPieceSamples.Any();

        // extraction is complete, when we have both pieces (current and next)
        private bool ExtractionComplete => _extractedPiece != null && _extractedNextPiece.HasValue;

        public TetrisAnalyzeState(TetrisAgent agent, Tetromino? currentTetromino = null)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));

            _agent = agent;
            _currentTetromino = currentTetromino;
            _extractedPieceSamples = new List<ProbabilisticResult<Piece>>(_agent.ExtractionSamples);
            _extractedNextPieceSamples = new List<ProbabilisticResult<Tetromino>>(_agent.ExtractionSamples);

            _agent.ExtractedPiece = null;
            _agent.ExtractedNextPiece = null;
            _agent.TracedPiece = null;
            _agent.ExpectedPiece = null;
        }

        public void Extract()
        {
            if (!_beginTime.HasValue)
            {
                // we can't do this in the constructor, because then we would use the previous screenshot's timestamp
                _beginTime = _agent.Screenshot.Timestamp;
            }

            int searchHeight = CalulateSearchHeight(_currentTetromino);
            _logger.Info($"Search height for extraction is {searchHeight}");

            ExtractGameState(searchHeight);
        }

        public void Play()
        {
            if (ExtractionComplete)
            {
                _logger.Info($"Game state extraction successfully:\n{_agent.GameState}");

                UpdateGlobalGameState();

                // perform the search
                // here we decide, where we want to place our tetromino on the board
                var results = Search();
                if (results == null) throw new Exception("search was not successful");

                _logger.Info("Agent found a solution");
                _logger.Info($"Solution: {string.Join(", ", results.Moves.Select(x => x.ToString()))}");

                // begin to execute the commands
                SetStateExecute(results);
            }
        }

        // this method return true, when the current and the next piece were extracted sucessfully
        // only then can we start the search and proceed to the execute-state
        private void ExtractGameState(int searchHeight)
        {
            ExtractCurrentPieceSampling(searchHeight);

            if (CanExtractNextPiece)
            {
                ExtractNextPieceSampling();
            }

            // TODO: remove!
            /*
            int searchHeight = CalulateSearchHeight(_currentTetromino);
            _logger.Info($"Search height for extraction is {searchHeight}");

            // just extract the current and next piece
            // we dont extract the board (too error prone), instead we carry along the game state

            if (_extractedPiece == null)
            {
                // extract the current piece
                var result = ExtractCurrentPiece(searchHeight);

                if (result.Result == null) return false;
                if (!result.Result.IsUntouched) return false; // spawned piece must be untouched

                _extractedPiece = result.Result;
                _extractedPieceTimestamp = _agent.Screenshot.Timestamp; // TODO: take time from clock or from the screenshot? make time diagram
                _agent.ExtractedPiece = _extractedPiece;
            }

            if (_extractedNextPiece == null)
            {
                // extract the next piece
                var result = ExtractNextPiece();

                if (result.Result == null) return false;

                _extractedNextPiece = result.Result;
                _agent.ExtractedNextPiece = _extractedNextPiece;
            }

            return true;*/
        }

        private void ExtractCurrentPieceSampling(int searchHeight)
        {
            // already extracted the piece?
            if (_extractedPiece != null) return;

            // extract the current piece
            var result = ExtractCurrentPiece(searchHeight);

            if (result.IsRejected(_agent.ExtractionLowerThreshold))
            {
                // reject (threshold not reached or piece is touched)
                _logger.Warn($"Reject extracted current piece (probability {result.Probability:F})");
                return;
            }
            if (!result.Result.IsUntouched)
            {
                // reject (threshold not reached or piece is touched)
                _logger.Warn($"Reject extracted current piece: not untouched ({result.Result}, probability {result.Probability:F})");
                return;
            }
            if (result.IsAccepted(_agent.ExtractionUpperThreshold))
            {
                // accept immediately
                _logger.Info($"Accept extracted current piece immediately ({result.Result}, probability {result.Probability:F})");
                AcceptCurrentPiece(result.Result);
                return;
            }

            // add sample
            _logger.Info($"Added sample for extracted current piece ({result.Result}, probability {result.Probability:F})");
            _extractedPieceSamples.Add(result);

            // do we have enougt samples?
            if (_extractedPieceSamples.Count >= _agent.ExtractionSamples)
            {
                // evaluate samples
                var samplesOrderedGrouped = _extractedPieceSamples
                    .GroupBy(x => x.Result, y => y.Probability)
                    .Select(x => new { Piece = x.Key, Number = x.Count(), ProbabilityAvg = x.Average(), ProbabilityMax = x.Max() })
                    .OrderByDescending(x => x.Number)
                    .ThenByDescending(x => x.ProbabilityAvg)
                    .ToList();

                _logger.Info($"Accept extracted current piece by sampling ({result.Result}, {samplesOrderedGrouped.First().Number} samples)");
                AcceptCurrentPiece(samplesOrderedGrouped.First().Piece);
            }
        }

        private void AcceptCurrentPiece(Piece currentPiece)
        {
            _extractedPiece = currentPiece;
            _extractedPieceTimestamp = _agent.Screenshot.Timestamp;
            _agent.ExtractedPiece = _extractedPiece;
        }
        
        private void ExtractNextPieceSampling()
        {
            // already extracted the piece?
            if (_extractedNextPiece != null) return;

            // extract the next piece
            var result = ExtractNextPiece();

            if (result.IsRejected(_agent.ExtractionLowerThreshold))
            {
                // reject (threshold not reached or piece is touched)
                _logger.Warn($"Reject extracted next piece (probability {result.Probability:F}, threshold {_agent.ExtractionLowerThreshold})");
                return;
            }
            if (result.IsAccepted(_agent.ExtractionUpperThreshold))
            {
                // accept immediately
                _logger.Info($"Accept extracted next piece immediately ({result.Result}, probability {result.Probability:F})");
                AcceptNextPiece(result.Result);
                return;
            }

            // add sample
            _logger.Info($"Added sample for extracted next piece ({result.Result}, probability {result.Probability:F})");
            _extractedNextPieceSamples.Add(result);

            // do we have enougt samples?
            if (_extractedNextPieceSamples.Count >= _agent.ExtractionSamples)
            {
                // evaluate samples
                var samplesOrderedGrouped = _extractedNextPieceSamples
                    .GroupBy(x => x.Result, y => y.Probability)
                    .Select(x => new { NextPiece = x.Key, Number = x.Count(), ProbabilityAvg = x.Average(), ProbabilityMax = x.Max() })
                    .OrderByDescending(x => x.Number)
                    .ThenByDescending(x => x.ProbabilityAvg)
                    .ToList();

                _logger.Info($"Accept extracted next piece by sampling ({result.Result}, {samplesOrderedGrouped.First().Number} samples)");
                AcceptNextPiece(samplesOrderedGrouped.First().NextPiece);
            }
        }
        
        private void AcceptNextPiece(Tetromino nextPiece)
        {
            _extractedNextPiece = nextPiece;
            _agent.ExtractedNextPiece = _extractedNextPiece;
        }

        private ProbabilisticResult<Piece> ExtractCurrentPiece(int searchHeight)
        {
            var screenshot = _agent.Screenshot;

            if (_currentTetromino.HasValue)
            {
                // we know which tetromino to look for
                // this gives us valuable information and we can validate our results
                return _agent.PieceExtractor.ExtractKnownPieceFuzzy(screenshot, new Piece(_currentTetromino.Value), searchHeight);
            }

            // this case only happens once (in the beginning of a new game)
            // we have to test every possible tetromino and take the most probable
            return _agent.PieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, searchHeight);
        }

        private ProbabilisticResult<Tetromino> ExtractNextPiece()
        {
            var screenshot = _agent.Screenshot;

            return _agent.PieceExtractor.ExtractNextPieceFuzzy(screenshot);
        }

        private void UpdateGlobalGameState()
        {
            _agent.GameState.Piece = _extractedPiece;
            _agent.GameState.NextPiece = _extractedNextPiece;
        }

        private SearchResult Search()
        {
            return _agent.Search.Search(_agent.GameState);
        }

        private int CalulateSearchHeight(Tetromino? tetromino)
        {
            if (!_beginTime.HasValue) throw new Exception("_beginTime is not initialized");

            // this is the time that passed since the next piece became visible
            var passedTime = _agent.Screenshot.Timestamp
                - _beginTime.Value
                + Timing.SleepAfterButtonTime
                + Timing.AnalyzeFallDurationPaddingTime;

            int searchHeightTime = TetrisLevel.GetMaxFallDistance(_agent.GameState.Level, passedTime);

            if (tetromino.HasValue)
            {
                // we know which tetromino we are looking for
                // we maximally search to the distance that this tetromino could possibly fall (drop distance)
                int dropDistance = _agent.GameState.Board.DropDistance(new Piece(tetromino.Value));
                return Math.Min(searchHeightTime, dropDistance);
            }

            // this case only happens once (in the beginning of a new game)
            // we don't know which tetromino we are looking for, so we take the maximum drop distance of every possible tetromino
            int maxDropDistance = _agent.GameState.Board.MaximumDropDistanceForSpawnedPiece();
            return Math.Min(searchHeightTime, maxDropDistance);
        }

        private void SetStateExecute(SearchResult results)
        {
            var moves = new Queue<Move>(results.Moves);
            var tracedPiece = new Piece(_agent.GameState.Piece);

            _agent.SetStateAndContinue(new TetrisExecuteState(_agent, moves, tracedPiece, _extractedPieceTimestamp));
        }
    }
}
