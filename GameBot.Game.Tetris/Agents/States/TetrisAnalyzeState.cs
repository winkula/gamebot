using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using NLog;
using System;
using System.Collections.Generic;

namespace GameBot.Game.Tetris.Agents.States
{
    public class TetrisAnalyzeState : ITetrisState
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private TetrisAgent _agent;

        private Tetromino? _currentTetromino;

        private Piece _extractedPiece;
        private Tetromino? _extractedNextPiece;

        private TimeSpan _timeNextAction = TimeSpan.Zero;
        
        public TetrisAnalyzeState(TetrisAgent agent, Tetromino? currentTetromino)
        {
            _agent = agent;

            _currentTetromino = currentTetromino;
        }

        public void Act()
        {
            // TODO: calculate duration as the time that passed since the initialization of this state
            var duration = TimeSpan.FromMilliseconds(Timing.ExpectedFallDurationPadding);
            int searchHeight = TetrisLevel.GetMaxFallDistance(_agent.GameState.StartLevel, duration);

            if (Extract(searchHeight))
            {
                // update global game state
                _agent.GameState.Piece = _extractedPiece;
                _agent.GameState.NextPiece = _extractedNextPiece;

                // we found a new piece. release the down key (end the drop)
                _agent.Executor.Release(Button.Down);
                _logger.Info("> End the drop.");

                // do the search
                // this is the essence of the a.i.
                var results = _agent.Search.Search(_agent.GameState);

                if (results != null)
                {
                    _logger.Info("Current game state:\n" + _agent.GameState);

                    _logger.Info("> AI found a solution.");
                    _logger.Info("> Goal game state:\n" + results.GoalGameState);
                    foreach (var move in results.Moves)
                    {
                        _logger.Info(move);
                    }

                    // somthing found.. we can execute now
                    Execute(results);
                }
            }
        }

        // this method return true, when the current and the next piece were extracted sucessfully
        // only then can we start the search and proceed to the execute-state
        private bool Extract(int searchHeight)
        {
            var screenshot = _agent.Screenshot;

            // we dont extract the board (too error prone)
            // instead we carry along the game state

            // extract the pieces
            // TODO: if currentTetromino.HasValue, then we know, which tetromino we look for, so we can optimize the piece matching
            _extractedPiece = _agent.Extractor.ExtractSpawnedPiece(screenshot, searchHeight);
            if (_extractedPiece == null) return false;
            if (_extractedPiece.Orientation != 0) return false; // spawned piece must have orientation 0
            if (_extractedPiece.X != 0) return false; // spawned piece must have x coordinate 0

            _extractedNextPiece = _agent.Extractor.ExtractNextPiece(screenshot);
            if (_extractedNextPiece == null) return false;

            if (_currentTetromino.HasValue && _currentTetromino.Value != _extractedPiece.Tetromino)
            {
                _logger.Info("> Extracted inconsistent current piece!");
                return false;
            }

            return true;
        }

        private void Execute(SearchResult results)
        {
            var moves = new Queue<Move>(results.Moves);
            _agent.SetState(new TetrisExecuteState(_agent, moves, _agent.Screenshot.Timestamp));
        }
    }
}
