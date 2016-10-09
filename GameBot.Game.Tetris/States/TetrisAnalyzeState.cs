using GameBot.Core.Data;
using GameBot.Game.Tetris.Agents;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameBot.Game.Tetris.States
{
    public class TetrisAnalyzeState : ITetrisState
    {
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
            // TODO: define search height
            //int searchHeight = TetrisLevel.GetMaxFallDistance();
            int searchHeight = 3;
            
            if (Extract(searchHeight))
            {
                // update global game state
                agent.GameState.Piece = extractedPiece;
                agent.GameState.NextPiece = extractedNextPiece;
                
                // we found a new piece. release the down key (end the drop)
                agent.Actuator.Release(Button.Down);
                Debug.WriteLine("> End the drop.");

                // do the search
                // this is the essence of the a.i.
                var results = agent.Search.Search(agent.GameState);

                if (results != null)
                {
                    Debug.WriteLine("> AI found a solution.");
                    Debug.WriteLine(results.GoalGameState);
                    foreach (var move in results.Moves)
                    {
                        Debug.WriteLine(move);
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
                Debug.WriteLine("> Extracted inconsistent current piece!");
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
