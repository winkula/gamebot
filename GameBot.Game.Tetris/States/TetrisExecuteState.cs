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

        public TetrisExecuteState(TetrisAgent agent, Queue<Move> moves, TimeSpan analyzeTimestamp)
        {
            this.agent = agent;

            this.moves = moves;
            this.lastPositionTimeStamp = analyzeTimestamp;

            if (agent.GameState == null) throw new ArgumentNullException(nameof(agent.GameState));
            if (agent.GameState.Piece == null) throw new ArgumentNullException(nameof(agent.GameState.Piece));
            if (agent.GameState.NextPiece == null) throw new ArgumentNullException(nameof(agent.GameState.NextPiece));

            this.lastPosition = new Piece(agent.GameState.Piece);
        }

        public void Act()
        {
            // first we have to check if the last command was successful
            if (lastMove.HasValue)
            {
                var now = agent.TimeProvider.Time;
                var duration = (now - lastPositionTimeStamp) + TimeSpan.FromSeconds(timePaddingSeconds);
                var expectedFallDistance = TetrisLevel.GetMaxFallDistance(agent.GameState.Level, duration);

                // TODO: remove this fix value
                expectedFallDistance = 10;

                Debug.WriteLine("> Check command. Maximal expected fall distance is " + expectedFallDistance);

                // TODO: solve the problem, when a I-piece spawns (maybe when its rotated and not the whole piece is visible?)
                var piece = agent.Extractor.ConfirmPieceMove(agent.Screenshot, lastPosition, lastMove.Value, expectedFallDistance);
                if (piece == null)
                {
                    Debug.WriteLine("> PIECE NOT FOUND! Looking for " + lastPosition.Tetromino + ". Try again.");
                    return;
                }

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

                // first execution
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
            agent.GameState.Piece = new Piece(lastPosition);
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

            // repeated execution
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
                case Move.Left:
                    agent.Actuator.Hit(Button.Left);
                    break;

                case Move.Right:
                    agent.Actuator.Hit(Button.Right);
                    break;

                case Move.Rotate:
                    agent.Actuator.Hit(Button.A); // clockwise rotation
                    break;

                case Move.RotateCounterclockwise:
                    agent.Actuator.Hit(Button.B); // counterclockwise rotation
                    break;

                case Move.Drop:
                    agent.Actuator.Press(Button.Down); // drop

                    // calculates the score and the level
                    agent.GameState.Drop();
                    break;

                default:
                    break;
            }
        }

        private void Analyze()
        {
            agent.SetState(new TetrisAnalyzeState(agent, agent.GameState.Piece.Tetromino));
        }
    }
}
