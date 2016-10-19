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

        private Piece _extractedPiece;
        private TimeSpan _extractedPieceTimestamp;
        private Tetromino? _extractedNextPiece;
        private TimeSpan? _beginTime;

        private bool _successfullyExtracted;

        public TetrisAnalyzeState(TetrisAgent agent, Tetromino? currentTetromino = null)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));

            _agent = agent;
            _currentTetromino = currentTetromino;

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

            _successfullyExtracted = ExtractGameState();
        }

        public void Play()
        {
            if (_successfullyExtracted)
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
            else
            {
                _logger.Info("Game state extraction skipped");
            }
        }

        // this method return true, when the current and the next piece were extracted sucessfully
        // only then can we start the search and proceed to the execute-state
        private bool ExtractGameState()
        {
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

            return true;
        }

        private ProbabilisticResult<Piece> ExtractCurrentPiece(int searchHeight)
        {
            var screenshot = _agent.Screenshot;

            if (_currentTetromino.HasValue)
            {
                // we know which tetromino to look for
                // this gives us valuable information and we can validate our results
                return _agent.PieceExtractor.ExtractKnownPieceFuzzy(screenshot, new Piece(_currentTetromino.Value), searchHeight, _agent.ProbabilityThreshold);
            }

            // this case only happens once (in the beginning of a new game)
            // we have to test every possible tetromino and take the most probable
            return _agent.PieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, searchHeight, _agent.ProbabilityThreshold);
        }

        private ProbabilisticResult<Tetromino?> ExtractNextPiece()
        {
            var screenshot = _agent.Screenshot;

            return _agent.PieceExtractor.ExtractNextPieceFuzzy(screenshot, _agent.ProbabilityThreshold);
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
