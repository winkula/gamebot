using GameBot.Core.Data;
using GameBot.Game.Tetris.Agents;
using GameBot.Game.Tetris.Data;
using System;
using System.Collections.Generic;

namespace GameBot.Game.Tetris.States
{
    public class TetrisAnalyzeState : ITetrisState
    {
        private TetrisAgent agent;

        private Tetromino? currentTetromino;

        private bool awaitNextTetromino = true;
        private TimeSpan timeNextAction = TimeSpan.Zero;

        private Queue<ICommand> commands = new Queue<ICommand>();

        public TetrisAnalyzeState(Tetromino? currentTetromino)
        {
            this.currentTetromino = currentTetromino;
        }

        public void Act(TetrisAgent agent)
        {
            this.agent = agent;

            Queue<ICommand> commands = new Queue<ICommand>();
            TetrisGameState currentGameState = Extract(3); // TODO: define search height

            if (currentTetromino.HasValue)
            {
                // we know which Tetromino to look for
            }
            else
            {
                // maybe the game just started, we must search in the board to find the current Tetromino 
            }
            
            if (MustPlay(currentGameState))
            {
                commands = new Queue<ICommand>(agent.Ai.Play(currentGameState));
                foreach (var command in commands)
                {
                    this.commands.Enqueue(command);
                }
                AfterPlay();
            }

            agent.SetState(new TetrisExecuteState(commands, currentGameState));
        }

        private TetrisGameState Extract(int searchHeight)
        {
            var screenshot = agent.Screenshot;

            var board = agent.Extractor.ExtractBoard(screenshot);
            var currentPiece = agent.Extractor.ExtractSpawnedPieceOrigin(screenshot);
            var nextPiece = agent.Extractor.ExtractNextPiece(screenshot);

            var gameState = new TetrisGameState(board, currentPiece, nextPiece);
            
            return gameState;
        }

        private bool MustPlay(TetrisGameState gameState)
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
