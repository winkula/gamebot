﻿using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using NLog;
using System;
using System.Linq;
using GameBot.Core.Exceptions;
using GameBot.Core.Extensions;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Extraction.Samplers;

namespace GameBot.Game.Tetris.Agents.States
{
    public class TetrisAnalyzeState : ITetrisAgentState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly TetrisAgent _agent;

        private readonly Tetrimino? _currentTetrimino;

        private readonly ISampler<Piece> _currentPieceSampler;
        private readonly ISampler<Tetrimino> _nextPieceSampler;

        private Piece _extractedPiece;
        private Tetrimino? _extractedNextPiece;
        private TimeSpan? _beginTime;
        private Board _extractedBoard;

        // we can extract the next piece only then, when we already have found the current piece
        private bool CanExtractNextPiece => _extractedPiece != null || _currentPieceSampler.SampleCount > 0;

        // extraction is complete, when we have both pieces (current and next)
        private bool ExtractionComplete => _extractedPiece != null && _extractedNextPiece.HasValue;

        public TetrisAnalyzeState(TetrisAgent agent, Tetrimino? currentTetrimino = null)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));

            _agent = agent;
            _currentTetrimino = currentTetrimino;
            _currentPieceSampler = new CurrentTetriminoSampler(_agent.ExtractionSamples);
            _nextPieceSampler = new NextTetriminoSampler(_agent.ExtractionSamples);

            _agent.ExtractedPiece = null;
            _agent.ExtractedNextPiece = null;
            _agent.TracedPiece = null;
        }

        public void Extract()
        {
            if (!_beginTime.HasValue)
            {
                // we can't do this in the constructor, because then we would use the previous screenshot's timestamp
                _beginTime = _agent.Screenshot.Timestamp;
            }

            int searchHeight = CalulateSearchHeight(_currentTetrimino);
            _agent.SearchHeight = searchHeight;
            _logger.Info($"Analyze (search height {searchHeight})");

            ExtractGameState(searchHeight);
        }

        public void Play()
        {
            if (ExtractionComplete)
            {
                UpdateGlobalGameState();

                _logger.Info($"Game state extraction successful\n{_agent.GameState}");

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

            ExtractCurrentPieceSampling(searchHeight);
            ExtractNextPieceSampling();
            ExtractBoard();
        }

        private void ExtractCurrentPieceSampling(int searchHeight)
        {
            // already extracted the piece?
            if (_extractedPiece != null) return;

            // extract the current piece
            var screenshot = _agent.Screenshot;
            var currentPiece = _agent.Extractor.ExtractCurrentPiece(screenshot, _currentTetrimino, searchHeight);

            if (currentPiece == null)
            {
                // reject (threshold not reached or piece is touched)
                _logger.Warn("Reject extracted current piece");
#if DEBUG
                _agent.Screenshot.Save(_agent.Quantizer, "reject_cp");
#endif
                return;
            }
            if (!currentPiece.IsUntouched)
            {
                // reject (threshold not reached or piece is touched)
                _logger.Warn($"Reject extracted current piece: not untouched ({currentPiece.Tetrimino})");
#if DEBUG
                _agent.Screenshot.Save(_agent.Quantizer, "reject_cp");
#endif
                return;
            }

            // add sample
            _logger.Info($"Added sample for extracted current piece ({currentPiece.Tetrimino})");
            _currentPieceSampler.Sample(new ProbabilisticResult<Piece>(currentPiece));
#if DEBUG
            _agent.Screenshot.Save(_agent.Quantizer, "sample_cp");
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
            _agent.ExtractedPiece = _extractedPiece;
        }

        private void ExtractNextPieceSampling()
        {
            if (!CanExtractNextPiece) return;

            // already extracted the piece?
            if (_extractedNextPiece != null) return;

            // extract the next piece
            var screenshot = _agent.Screenshot;
            var nextPiece = _agent.Extractor.ExtractNextPiece(screenshot);

            if (!nextPiece.HasValue)
            {
                // reject (threshold not reached or piece is touched)
                _logger.Warn("Reject extracted next piece");
#if DEBUG
                _agent.Screenshot.Save(_agent.Quantizer, "reject_np");
#endif
                return;
            }

            // add sample
            _logger.Info($"Added sample for extracted next piece ({nextPiece})");
            _nextPieceSampler.Sample(new ProbabilisticResult<Tetrimino>(nextPiece.Value));
#if DEBUG
            _agent.Screenshot.Save(_agent.Quantizer, "sample_np");
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
            _agent.ExtractedNextPiece = _extractedNextPiece;
        }

        private void ExtractBoard()
        {
            if (_agent.PlayMultiplayer)
            {
                // recognize if lines are spawned from the bottom
                _agent.GameState.Board = _agent.BoardExtractor.UpdateMultiplayer(_agent.Screenshot, _agent.GameState.Board);
            }

            if (_agent.CheckEnabled)
            {
                if (_extractedPiece == null) return;
                if (_agent.BoardExtractor.IsHorizonBroken(_agent.Screenshot, _agent.GameState.Board))
                {
                    _logger.Info("Extract board");

                    _extractedBoard = _agent.BoardExtractor.Update(_agent.Screenshot, _agent.GameState.Board, _extractedPiece);
                }
            }
        }

        private void UpdateGlobalGameState()
        {
            _agent.GameState.Piece = _extractedPiece;
            _agent.GameState.NextPiece = _extractedNextPiece;
            if (_extractedBoard != null)
            {
                _agent.GameState.Board = _extractedBoard;
            }
        }

        private SearchResult Search()
        {
            return _agent.Search.Search(_agent.GameState);
        }

        private int CalulateSearchHeight(Tetrimino? tetrimino)
        {
            if (!_beginTime.HasValue) throw new Exception("_beginTime is not initialized");

            // this is the time that passed since the next piece became visible
            var passedTime = _agent.Screenshot.Timestamp
                - _beginTime.Value
                + _agent.AnalyzePaddingTime;

            int searchHeightTime = TetrisLevel.GetFallDistance(_agent.GameState.Level, passedTime, _agent.GameState.HeartMode);

            if (tetrimino.HasValue)
            {
                // we know which tetrimino we are looking for
                // we maximally search to the distance that this tetrimino could possibly fall (drop distance)
                int dropDistance = _agent.GameState.Board.DropDistance(new Piece(tetrimino.Value));
                return Math.Min(searchHeightTime, dropDistance);
            }

            // this case only happens once (in the beginning of a new game)
            // we don't know which tetrimino we are looking for, so we take the maximum drop distance of every possible tetrimino
            int maxDropDistance = _agent.GameState.Board.MaximumDropDistanceForSpawnedPiece();
            return Math.Min(searchHeightTime, maxDropDistance);
        }

        private void SetStateExecute(SearchResult results)
        {
            var moves = results.Moves.ToList();
            var tracedPiece = new Piece(_agent.GameState.Piece);

            _agent.SetStateAndContinue(new TetrisExecuteAllState(_agent, moves, tracedPiece));
        }
    }
}
