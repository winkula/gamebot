using System;
using System.Linq;
using GameBot.Core.Exceptions;
using GameBot.Core.Extensions;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Extraction.Samplers;
using GameBot.Game.Tetris.Searching;
using NLog;

namespace GameBot.Game.Tetris.States
{
    public class AnalyzeState : BaseState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private Tetrimino? _currentTetrimino;

        private readonly ISampler<Piece> _currentPieceSampler;
        private readonly ISampler<Tetrimino> _nextPieceSampler;

        private readonly TimeSpan _beginTime;
        private Piece _extractedPiece;
        private Tetrimino? _extractedNextPiece;

        private bool _boardChecked;

        // we can extract the next piece only then, when we already have found the current piece
        private bool CanExtractNextPiece => _extractedPiece != null || _currentPieceSampler.SampleCount > 0;

        // extraction is complete, when we have both pieces (current and next)
        private bool ExtractionComplete => _extractedPiece != null && _extractedNextPiece.HasValue;

        public AnalyzeState(TetrisAgent agent, TimeSpan beginTime, Tetrimino? currentTetrimino = null) : base(agent)
        {
            _beginTime = beginTime;
            _currentTetrimino = currentTetrimino;
            _currentPieceSampler = new CurrentTetriminoSampler(Agent.ExtractionSamples);
            _nextPieceSampler = new NextTetriminoSampler(Agent.ExtractionSamples);

            Agent.ExtractedPiece = null;
            Agent.ExtractedNextPiece = null;
            Agent.TracedPiece = null;
        }

        public override void Extract()
        {
            // detect game over
            base.Extract();

            int searchHeight = CalulateSearchHeight(_currentTetrimino);
            Agent.SearchHeight = searchHeight;
            _logger.Info($"Analyze (search height {searchHeight})");

            ExtractGameState(searchHeight);
        }

        public override void Play()
        {
            if (ExtractionComplete)
            {
                UpdateGlobalGameState();

                _logger.Info($"Game state extraction successful\n{Agent.GameState}");

                // perform the search
                // here we decide, where we want to place our tetrimino on the board
                var results = Search();
                if (results == null) throw new Exception("search was not successful");

                _logger.Info($"Agent found a solution: {string.Join(", ", results.Moves.Select(x => x.ToString()))}");

                // begin to execute the commands
                SetStateExecute(results);
            }
        }

        // this method return true, when the current and the next piece were extracted sucessfully
        // only then can we start the search and proceed to the execute-state
        private void ExtractGameState(int searchHeight)
        {
            if (searchHeight < 0) throw new GameOverException("Search height is negative");

            /*
            if (Agent.Extractor.DetectPiece(Screenshot, searchHeight))
            {
                //if (!_boardChecked)
                {
                    // we only want to check the board once per analyze phase
                    ExtractBoard(searchHeight);
                    //_boardChecked = true;
                }
            }
            */

            ExtractCurrentPieceSampling(searchHeight);
            ExtractNextPieceSampling();
            ExtractBoard(searchHeight);
        }

        private void ExtractBoard(int searchHeight)
        {
            if (Agent.IsMultiplayer)
            {
                if (_extractedPiece == null) return;

                // recognize if lines are spawned from the bottom
                // TODO: catch exception of make further pre checks
                _logger.Info("Update board in multiplayer mode");
                Agent.GameState.Board = Agent.BoardExtractor.UpdateMultiplayer(Screenshot, Agent.GameState.Board);
            }

            if (Agent.CheckEnabled)
            {
                if (Agent.BoardExtractor.IsHorizonBroken(Screenshot, Agent.GameState.Board))
                {
                    // we have to newly recoginze the current piece
                    // maybe we missed one and the assumption over the current piece will be wrong
                    var piece = Agent.Extractor.ExtractCurrentPiece(Screenshot, null, searchHeight);
                    if (piece != null)
                    {
                        _logger.Info("Game state maybe broken. Analyze board");

                        _currentTetrimino = piece.Tetrimino;

                        // TODO: update board in method UpdateGlobalGameState
                        Agent.GameState.Board = Agent.BoardExtractor.Update(Screenshot, Agent.GameState.Board, piece);
                    }
                }
            }
        }

        private void ExtractCurrentPieceSampling(int searchHeight)
        {
            // already extracted the piece?
            if (_extractedPiece != null) return;

            // extract the current piece
            var screenshot = Screenshot;
            var currentPiece = Agent.Extractor.ExtractCurrentPiece(screenshot, _currentTetrimino, searchHeight);

            if (currentPiece == null)
            {
                // reject (threshold not reached or piece is touched)
                _logger.Warn("Reject extracted current piece");
#if DEBUG
                Screenshot.Save(Agent.Quantizer, "reject_cp");
#endif
                return;
            }
            if (!currentPiece.IsUntouched)
            {
                // reject (threshold not reached or piece is touched)
                _logger.Warn($"Reject extracted current piece: not untouched ({currentPiece.Tetrimino})");
#if DEBUG
                Screenshot.Save(Agent.Quantizer, "reject_cp");
#endif
                return;
            }

            // add sample
            _logger.Info($"Added sample for extracted current piece ({currentPiece.Tetrimino})");
            _currentPieceSampler.Sample(new ProbabilisticResult<Piece>(currentPiece));
#if DEBUG
            Screenshot.Save(Agent.Quantizer, "sample_cp");
#endif
            if (_currentPieceSampler.IsComplete)
            {
                // we have enought samples
                // evaluate our "true" result
                var acceptedCurrentPiece = _currentPieceSampler.Result;

                _logger.Info($"Accept extracted current piece by sampling ({currentPiece.Tetrimino})");
                AcceptCurrentPiece(acceptedCurrentPiece);
            }
        }

        private void AcceptCurrentPiece(Piece currentPiece)
        {
            _extractedPiece = currentPiece;
            Agent.ExtractedPiece = _extractedPiece;
        }

        private void ExtractNextPieceSampling()
        {
            if (!CanExtractNextPiece) return;

            // already extracted the piece?
            if (_extractedNextPiece != null) return;

            // extract the next piece
            var screenshot = Screenshot;
            var nextPiece = Agent.Extractor.ExtractNextPiece(screenshot);

            if (!nextPiece.HasValue)
            {
                // reject (threshold not reached or piece is touched)
                _logger.Warn("Reject extracted next piece");
#if DEBUG
                Screenshot.Save(Agent.Quantizer, "reject_np");
#endif
                return;
            }

            // add sample
            _logger.Info($"Added sample for extracted next piece ({nextPiece})");
            _nextPieceSampler.Sample(new ProbabilisticResult<Tetrimino>(nextPiece.Value));
#if DEBUG
            Screenshot.Save(Agent.Quantizer, "sample_np");
#endif
            if (_nextPieceSampler.IsComplete)
            {
                // we have enought samples
                // evaluate our "true" result
                var acceptedNextPiece = _nextPieceSampler.Result;

                _logger.Info($"Accept extracted next piece by sampling ({acceptedNextPiece})");
                AcceptNextPiece(acceptedNextPiece);
            }
        }

        private void AcceptNextPiece(Tetrimino nextPiece)
        {
            _extractedNextPiece = nextPiece;
            Agent.ExtractedNextPiece = _extractedNextPiece;
        }

        private void UpdateGlobalGameState()
        {
            Agent.GameState.Piece = _extractedPiece;
            Agent.GameState.NextPiece = _extractedNextPiece;
        }

        private SearchResult Search()
        {
            return Agent.Search.Search(Agent.GameState);
        }

        private int CalulateSearchHeight(Tetrimino? tetrimino)
        {
            // this is the time that passed since the next piece became visible
            var passedTime = Screenshot.Timestamp
                - _beginTime
                + Agent.MoreTimeToAnalyze;

            int searchHeightTime = (int)Math.Ceiling(TetrisLevel.GetFallDistance(Agent.GameState.Level, passedTime, Agent.GameState.HeartMode));

            if (tetrimino.HasValue)
            {
                // we know which tetrimino we are looking for
                // we maximally search to the distance that this tetrimino could possibly fall (drop distance)
                int dropDistance = Agent.GameState.Board.DropDistance(new Piece(tetrimino.Value));
                return Math.Min(searchHeightTime, dropDistance);
            }

            // this case only happens once (in the beginning of a new game)
            // we don't know which tetrimino we are looking for, so we take the maximum drop distance of every possible tetrimino
            int maxDropDistance = Agent.GameState.Board.MaximumDropDistanceForSpawnedPiece();
            return Math.Min(searchHeightTime, maxDropDistance);
        }

        private void SetStateExecute(SearchResult results)
        {
            var moves = results.Moves.ToList();
            var tracedPiece = new Piece(Agent.GameState.Piece);

            SetStateAndContinue(new ExecuteState(Agent, moves, tracedPiece));
        }
    }
}
