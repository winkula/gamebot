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

        private GameState currentGameState;

        private Tetromino? currentTetromino;
        private bool awaitNextTetromino = true;
        private TimeSpan timeNextAction = TimeSpan.Zero;

        public TetrisAnalyzeState(Tetromino? currentTetromino)
        {
            this.currentTetromino = currentTetromino;
        }

        public void Act(TetrisAgent agent)
        {
            this.agent = agent;

            // TODO: define search height
            //int searchHeight = TetrisLevel.GetMaxFallDistance();
            int searchHeight = 3;

            // TODO: always carry along the game state! only clear when a new game starts
            currentGameState = Extract(searchHeight);
            if (currentGameState != null && currentGameState.Piece != null)
            {
                // we found a new piece. release the down key (end the drop)
                agent.Actuator.Release(Button.Down);
                Debug.WriteLine("> End the drop.");

                // do the search
                // this is the essence of the a.i.
                var results = agent.Search.Search(currentGameState);

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

        private GameState Extract(int searchHeight)
        {
            if (currentTetromino.HasValue)
            {
                // we know which Tetromino to look for
            }
            else
            {
                // maybe the game just started, we must search in the board to find the current Tetromino 
            }

            var screenshot = agent.Screenshot;

            var board = agent.Extractor.ExtractBoard(screenshot);
            var currentPiece = agent.Extractor.ExtractSpawnedPiece(screenshot, searchHeight);
            var nextPiece = agent.Extractor.ExtractNextPiece(screenshot);

            var gameState = new GameState(board, currentPiece, nextPiece);

            return gameState;
        }

        private void Execute(SearchResult results)
        {
            var moves = new Queue<Move>(results.Moves);
            agent.SetState(new TetrisExecuteState(moves, currentGameState, agent.Screenshot.Timestamp));
        }

        private bool MustPlay(GameState gameState)
        {
            if (gameState == null) throw new ArgumentNullException(nameof(gameState));

            if (gameState.Piece == null && timeNextAction <= agent.TimeProvider.Time)
            {
                // await next tetromino when the piece was null some time in the past
                // and the timer to look for the next piece is exceeded
                awaitNextTetromino = true;
            }

            agent.Debugger.WriteStatic(agent.Ai.CurrentGameState);

            return awaitNextTetromino &&
                gameState.Piece != null &&
                gameState.NextPiece != null;
        }

        public void AfterPlay()
        {
            // start next timer
            var duration = TetrisLevel.GetFreeFallDuration(agent.Ai.LastWay.Fall);

            timeNextAction = agent.TimeProvider.Time.Add(duration);
            awaitNextTetromino = false;
        }
    }
}
