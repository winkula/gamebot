using GameBot.Core.Data;
using GameBot.Game.Tetris.Agents;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GameBot.Game.Tetris.States
{
    public class TetrisExecuteState : ITetrisState
    {
        private const double timePaddingSeconds = 0.4;

        private TetrisAgent agent;

        private Queue<Move> moves;

        private Move? lastMove;
        private Piece lastPosition;
        private TimeSpan lastPositionTimeStamp;

        private GameState currentGameState;

        public TetrisExecuteState(Queue<Move> moves, GameState currentGameState, TimeSpan analyzeTimetampt)
        {
            this.moves = moves;
            this.currentGameState = currentGameState;
            if (currentGameState.Piece != null)
            {
                UpdateLastPosition(currentGameState.Piece, analyzeTimetampt);
            }
        }

        public void Act(TetrisAgent agent)
        {
            this.agent = agent;

            // first we have to check if the last command was successful
            if (lastMove.HasValue)
            {
                var now = agent.TimeProvider.Time;
                var duration = (now - lastPositionTimeStamp) + TimeSpan.FromSeconds(timePaddingSeconds);
                var expectedFallDistance = TetrisLevel.GetMaxFallDistance(currentGameState.Level, duration);
                Debug.WriteLine("> Check command. Maximal expected fall distance is " + expectedFallDistance);
                
                var piece = agent.Extractor.ConfirmPieceMove(agent.Screenshot, lastPosition, lastMove.Value, expectedFallDistance);
                if (piece == null)
                    throw new ApplicationException("we have not found the piece! we can't say if the command was executed or not!");
                                
                // check if last command was executed
                // if not, repeat      
                var delta = piece.Delta(new Piece(lastPosition).Apply(lastMove.Value));

                UpdateLastPosition(piece, now);
                if (delta.IsTargetPosition)
                {
                    // move was successfully executed
                    // we remove it from the queue
                    NextCommand();
                }
                else
                {
                    // the command was not executed and the tile is in the old position
                    Debug.WriteLine("> Failed to execute the command.");
                    RepeatCommand();
                    return; // we return here because we need a new screenshot
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

        private void UpdateLastPosition(Piece newLastPosition, TimeSpan newLastPositionTimestamp)
        {
            lastPosition = newLastPosition;
            lastPositionTimeStamp = newLastPositionTimestamp;
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
                NextCommand(); // we can already remove this, beacause we don't check it later
            }

            Debug.WriteLine("> Execute " + move);
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
            agent.SetState(new TetrisAnalyzeState(currentGameState.NextPiece));
        }
    }
}
