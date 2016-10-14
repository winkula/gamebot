using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris.Agents.States
{
    public class TetrisExecuteState : ITetrisState
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static Logger loggerCamera = LogManager.GetLogger("Stats.Camera");
        private static Logger loggerActuator = LogManager.GetLogger("Stats.Actuator");

        private TetrisAgent agent;

        private Queue<Move> moves;

        private Move? lastMove;
        private Piece lastPosition;
        private TimeSpan lastPositionTimeStamp;

        public TetrisExecuteState(TetrisAgent agent, Queue<Move> moves, TimeSpan analyzeTimestamp)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            if (moves == null) throw new ArgumentNullException(nameof(moves));
            if (analyzeTimestamp == null) throw new ArgumentNullException(nameof(analyzeTimestamp));

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
                var now = agent.Clock.Time;
                var expectedFallDistance = GetExpectedFallDistance(now);
                logger.Info("> Check command. Maximal expected fall distance is " + expectedFallDistance);
                                
                var piece = agent.Extractor.ExtractMovedPieceWithErrorTolerance(agent.Screenshot, lastPosition, lastMove.Value, expectedFallDistance);
                if (piece == null)
                {
                    logger.Info("> PIECE NOT FOUND! Looking for " + lastPosition.Tetromino + ". Try again.");
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
                    ProceedToNextCommand();
                }
                else
                {
                    // the command was not executed and the tile is in the old position
                    logger.Info("> Failed to execute the command.");
                    RepeatCommand();
                    return; // we return here because we need a new screenshot
                }
            }

            // are there commands to execute?
            if (moves.Any())
            {
                var move = moves.Peek();

                // first execution
                if (move != Move.Drop)
                {
                    ExecuteAndCheck(move);
                }
                else
                {
                    if (moves.Count != 1)
                        throw new Exception("Drop must be the last move to execute.");

                    ExecuteWithoutCheck(move);
                }

                if (!moves.Any() && !lastMove.HasValue)
                {
                    // we executed all moves and have no comannd to check
                    // back to the analyze state
                    Analyze();
                }
            }
        }

        private int GetExpectedFallDistance(TimeSpan now)
        {
            // we add some time to the theoretical duration between now and the
            // timestamp of the last analyzed screenshot
            // so we are sure, that we don't miss the piece
            var duration = 
                (now - lastPositionTimeStamp) + 
                TimeSpan.FromMilliseconds(Timing.ExpectedFallDurationPadding);

            return TetrisLevel.GetMaxFallDistance(agent.GameState.Level, duration);
        }

        private void UpdateLastPosition(Piece newLastPosition, TimeSpan newLastPositionTimestamp)
        {
            lastPosition = newLastPosition;
            agent.GameState.Piece = new Piece(lastPosition);
            lastPositionTimeStamp = newLastPositionTimestamp;
        }

        private void ProceedToNextCommand()
        {
            moves.Dequeue();
        }

        private void RepeatCommand()
        {
            if (!lastMove.HasValue)
                throw new ArgumentNullException(nameof(lastMove));

            // repeated execution
            ExecuteAndCheck(lastMove.Value);
        }

        private void ExecuteAndCheck(Move move)
        {
            lastMove = move;

            Execute(move);
        }

        private void ExecuteWithoutCheck(Move move)
        {
            lastMove = null;
            ProceedToNextCommand();

            Execute(move);
        }

        private void Execute(Move move)
        {
            logger.Info("> Execute " + move);
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
                    var dropDistance = agent.GameState.Drop();

                    // let time lapse away
                    // (the tetromino falls and we must pause
                    // TODO: here we could to some precalculations for the next search???
                    AwaitDrop(dropDistance);
                    break;

                default:
                    break;
            }
        }

        private void AwaitDrop(int fallDistanceRows)
        {
            // we subtract a time padding, because we dont want to wait the
            // theoretical drop duration, but the real drop duration
            // (we lose some overhead time)  
            var dropDuration = TetrisLevel
                .GetFreeFallDuration(fallDistanceRows)
                .Subtract(TimeSpan.FromMilliseconds(Timing.NegativeDropDurationPadding));

            if (dropDuration > TimeSpan.Zero)
            {
                agent.Clock.Sleep((int)dropDuration.TotalMilliseconds);
            }
        }

        private void Analyze()
        {
            agent.SetState(new TetrisAnalyzeState(agent, agent.GameState.Piece.Tetromino));
        }
    }
}
