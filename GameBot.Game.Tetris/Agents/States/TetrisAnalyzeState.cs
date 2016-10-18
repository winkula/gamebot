using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris.Agents.States
{
    public class TetrisAnalyzeState : ITetrisAgentState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly TetrisAgent _agent;

        private Tetromino? _currentTetromino;

        private Piece _extractedPiece;
        private Tetromino? _extractedNextPiece;

        // should never be overestimated!
        private readonly TimeSpan _beginTime;

        public TetrisAnalyzeState(TetrisAgent agent, Tetromino? currentTetromino)
        {
            _agent = agent;

            _currentTetromino = currentTetromino;
            _beginTime = _agent.Clock.Time;
        }

        public void Act()
        {
            if (Extract())
            {
                // update global game state
                _agent.GameState.Piece = _extractedPiece;
                _agent.ExtractedPiece = _extractedPiece;
                _agent.GameState.NextPiece = _extractedNextPiece;
                _agent.ExtractedNextPiece = _extractedNextPiece;

                _logger.Info($"Game state extraction successfully:\n{_agent.GameState}");

                // we found a new piece. release the down key (end the drop)
                //_agent.Executor.Release(Button.Down);
                //_logger.Info("> End the drop.");

                // do the search
                // this is the essence of the a.i.
                var results = _agent.Search.Search(_agent.GameState);
                if (results != null)
                {
                    _logger.Info("A.I. found a solution");
                    _logger.Info($"Solution: {string.Join(", ", results.Moves.Select(x => x.ToString()))}");

                    // we can execute now
                    SetStateExecute(results);
                }
            }
            else
            {
                _logger.Info("Game state extraction skipped");
                _agent.ExtractedPiece = null;
                _agent.ExtractedNextPiece = null;
            }
        }

        // this method return true, when the current and the next piece were extracted sucessfully
        // only then can we start the search and proceed to the execute-state
        private bool Extract()
        {
            int searchHeight = CalulateSearchHeight(_currentTetromino);
            _logger.Info($"Search height for extraction is {searchHeight}");

            var screenshot = _agent.Screenshot;

            // we dont extract the board (too error prone)
            // instead we carry along the game state

            // extract the current piece
            var result = _currentTetromino.HasValue ?
                _agent.PieceExtractor.ExtractKnownPieceFuzzy(screenshot, new Piece(_currentTetromino.Value), searchHeight, _agent.ProbabilityThreshold) :
                _agent.PieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, searchHeight, _agent.ProbabilityThreshold);

            if (result.Result == null) return false;
            _extractedPiece = result.Result;
            if (_extractedPiece.Orientation != 0) return false; // spawned piece must have orientation 0
            if (_extractedPiece.X != 0) return false; // spawned piece must have x coordinate 0

            // extract the next piece
            var resultNextPiece = _agent.PieceExtractor.ExtractNextPieceFuzzy(screenshot, _agent.ProbabilityThreshold);
            _extractedNextPiece = resultNextPiece.Result;
            if (_extractedNextPiece == null) return false;

            return true;
        }

        private int CalulateSearchHeight(Tetromino? tetromino)
        {
            var passedTime = (_agent.Clock.Time - _beginTime)
                + TimeSpan.FromMilliseconds(Timing.PaddingAnalyze);

            int searchHeightTime = TetrisLevel.GetMaxFallDistance(_agent.GameState.Level, passedTime);

            if (tetromino.HasValue)
            {
                int dropDistance = _agent.GameState.Board.DropDistance(new Piece(tetromino.Value));
                return Math.Min(searchHeightTime, dropDistance);
            }

            int maxDropDistance = _agent.GameState.Board.MaxDropDistanceForSpawnedPiece();
            return Math.Min(searchHeightTime, maxDropDistance);
        }

        private void SetStateExecute(SearchResult results)
        {
            var moves = new Queue<Move>(results.Moves);
            _agent.SetState(new TetrisExecuteState(_agent, moves, _agent.Screenshot.Timestamp));
        }
    }
}
