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
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private TetrisAgent agent;

        private Tetromino? currentTetromino;

        private Piece extractedPiece;
        private Tetromino? extractedNextPiece;

        private TimeSpan timeNextAction = TimeSpan.Zero;
        
        public TetrisAnalyzeState(TetrisAgent agent, Tetromino? currentTetromino)
        {
            this.agent = agent;

            this.currentTetromino = currentTetromino;
        }

        public void Act()
        {
            // TODO: calculate duration as the time that passed since the initialization of this state
            var duration = TimeSpan.FromMilliseconds(Timing.ExpectedFallDurationPadding);
            int searchHeight = TetrisLevel.GetMaxFallDistance(agent.GameState.StartLevel, duration);

            if (Extract(searchHeight))
            {
                // update global game state
                agent.GameState.Piece = extractedPiece;
                agent.GameState.NextPiece = extractedNextPiece;

                // we found a new piece. release the down key (end the drop)
                agent.Executor.Release(Button.Down);
                logger.Info("> End the drop.");

                // do the search
                // this is the essence of the a.i.
                var results = agent.Search.Search(agent.GameState);

                if (results != null)
                {
                    logger.Info("> AI found a solution.");
                    logger.Info("> Goal game state:\n" + results.GoalGameState);
                    foreach (var move in results.Moves)
                    {
                        logger.Info(move);
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
            var screenshot = agent.Screenshot;

            // we dont extract the board (too error prone)
            // instead we carry along the game state

            // extract the pieces
            // TODO: if currentTetromino.HasValue, then we know, which tetromino we look for, so we can optimize the piece matching
            extractedPiece = agent.Extractor.ExtractSpawnedPiece(screenshot, searchHeight);
            if (extractedPiece == null) return false;
            if (extractedPiece.Orientation != 0) return false; // spawned piece must have orientation 0
            if (extractedPiece.X != 0) return false; // spawned piece must have x coordinate 0

            extractedNextPiece = agent.Extractor.ExtractNextPiece(screenshot);
            if (extractedNextPiece == null) return false;

            if (currentTetromino.HasValue && currentTetromino.Value != extractedPiece.Tetromino)
            {
                logger.Info("> Extracted inconsistent current piece!");
                return false;
            }

            return true;
        }

        private void Execute(SearchResult results)
        {
            var moves = new Queue<Move>(results.Moves);
            agent.SetState(new TetrisExecuteState(agent, moves, agent.Screenshot.Timestamp));
        }
    }
}
