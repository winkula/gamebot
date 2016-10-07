using GameBot.Core.Data;
using GameBot.Game.Tetris.Agents;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris.States
{
    public class TetrisExecuteState : ITetrisState
    {
        private const double timePadding = 0.4;

        private TetrisAgent agent;

        private Queue<Move> moves;
        private Move? lastMove;
        private Piece lastPosition;
        private Piece expectedPosition;
        private TimeSpan screenshotTimestamp;

        private GameState currentGameState;
        private Tetromino? nextTetromino;

        public TetrisExecuteState(Queue<Move> moves, GameState currentGameState, TimeSpan analyzeTimetampt)
        {
            this.moves = moves;
            this.currentGameState = currentGameState;
            this.nextTetromino = currentGameState.NextPiece;
            this.screenshotTimestamp = analyzeTimetampt;
            if (currentGameState.Piece != null)
            {
                this.lastPosition = new Piece(currentGameState.Piece);
            }
        }

        public void Act(TetrisAgent agent)
        {
            this.agent = agent;

            // first we have to check if the last command was successful
            if (lastMove.HasValue)
            {
                var now = agent.TimeProvider.Time;
                var duration = (now - screenshotTimestamp) + TimeSpan.FromSeconds(timePadding);
                var expectedFallDistance = TetrisLevel.GetFallDistance(currentGameState.Level, duration);

                // TODO: calculate fall disatnce correct
                expectedFallDistance = 10;

                var piece = agent.Extractor.ConfirmPieceMove(agent.Screenshot, lastPosition, lastMove.Value, expectedFallDistance);
                if (piece == null)
                    throw new ApplicationException("we have not found the piece! we can't say if the command was executed or not!");

                lastPosition = piece;
                var delta = piece.Delta(new Piece(lastPosition).Apply(lastMove.Value));

                // check if last command was executed
                // if not, repeat                
                if (delta.IsTargetPosition)
                {
                    // move was successfully executed
                    // we remove it from the queue
                    NextCommand();
                }
                else
                {
                    RepeatCommand();
                    return;
                }
            }

            // are there commands to execute?
            if (moves != null && moves.Any())
            {
                var move = moves.Peek();

                bool check = move != Move.Drop;
                Execute(move, check);
            }
            else
            {
                // we executed all moves
                // back to the analyze state
                Analyze();
            }
        }

        private void NextCommand()
        {
            moves.Dequeue();
        }

        private void RepeatCommand()
        {
            if (!lastMove.HasValue)
                throw new ArgumentNullException(nameof(lastMove));

            Execute(lastMove.Value, true);
        }

        private void Execute(Move move, bool check)
        {
            if (check)
            {
                lastMove = move;
            }
            else
            {
                lastMove = null;
            }

            switch (move)
            {
                case Move.Left: agent.Actuator.Hit(Button.Left); break;
                case Move.Right: agent.Actuator.Hit(Button.Right); break;
                case Move.Rotate: agent.Actuator.Hit(Button.A); break; // clockwise rotation
                case Move.RotateCounterclockwise: agent.Actuator.Hit(Button.B); break; // counterclockwise rotation
                case Move.Drop: agent.Actuator.Press(Button.Down); break; // drop
                default: break;
            }
        }

        private void Analyze()
        {
            agent.SetState(new TetrisAnalyzeState(nextTetromino));
        }
    }
}
