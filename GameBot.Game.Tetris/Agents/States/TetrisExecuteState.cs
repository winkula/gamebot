using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris.Agents.States
{
    public class TetrisExecuteState : ITetrisAgentState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly TetrisAgent _agent;

        private readonly Queue<Move> _pendingMoves;
        
        private readonly Piece _tracedPiece;

        public TetrisExecuteState(TetrisAgent agent, Queue<Move> pendingMoves, Piece tracedPiece)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            if (pendingMoves == null) throw new ArgumentNullException(nameof(pendingMoves));
            if (!pendingMoves.Any()) throw new ArgumentException("pendingMoves contains no elements");
            if (tracedPiece == null) throw new ArgumentNullException(nameof(tracedPiece));

            _agent = agent;
            _pendingMoves = pendingMoves;
            _tracedPiece = tracedPiece;

            if (_agent.GameState == null) throw new ArgumentNullException(nameof(agent.GameState));
            if (_agent.GameState.Piece == null) throw new ArgumentNullException(nameof(agent.GameState.Piece));
            if (_agent.GameState.NextPiece == null) throw new ArgumentNullException(nameof(agent.GameState.NextPiece));
        }

        public void Extract()
        {
            // do nothing
        }

        public void Play()
        {
            // next move to execute
            var move = _pendingMoves.Dequeue();

            if (move == Move.Drop)
            {
                if (_pendingMoves.Any()) throw new Exception("Drop must be the last move to execute");
                
                ExecuteDrop();
                SetStateAnalyze();
            }
            else
            {
                Execute(move);
                SetStateCheck(move);
            }
        }

        private void ExecuteDrop()
        {
            // calculates drop distance, score and new level
            int linesBefore = _agent.GameState.Lines;
            var dropDistance = _agent.GameState.Drop();
            var dropDuration = TetrisTiming.GetDropDuration(dropDistance);
            if (_agent.GameState.Lines > linesBefore)
            {
                // lines were removed, add extra time
                var lineRemoveDuration = TetrisTiming.GetLineRemovingDuration();
                dropDuration += lineRemoveDuration;
            }

            // we subtract a time padding, because we dont want to wait the
            // theoretical drop duration, but the real drop duration
            // (we lose some overhead time)  
            var waitDuration = dropDuration - TimeSpan.FromMilliseconds(Timing.DropDurationPadding);

            // execute the drop blocking
            // we must wait until the drop is ended before we can continue
            // TODO: here we could to some precalculations for the next search (and execute the drop asynchronous)???
            _agent.Executor.Hit(Button.Down, waitDuration);
        }
        
        private void Execute(Move move)
        {
            _logger.Info("Execute " + move);
            switch (move)
            {
                case Move.Left:
                    _agent.Executor.Hit(Button.Left);
                    break;

                case Move.Right:
                    _agent.Executor.Hit(Button.Right);
                    break;

                case Move.Rotate:
                    _agent.Executor.Hit(Button.A); // clockwise rotation
                    break;

                case Move.RotateCounterclockwise:
                    _agent.Executor.Hit(Button.B); // counterclockwise rotation
                    break;

                case Move.Drop:
                    throw new ApplicationException("no drop allowed here!");
            }
        }

        private void SetStateCheck(Move lastMove)
        {
            if (lastMove == Move.Drop) throw new Exception("Can't check drop");

            _agent.SetState(new TetrisCheckState(_agent, lastMove, _pendingMoves, _tracedPiece));
        }

        private void SetStateAnalyze()
        {
            if (_pendingMoves.Any()) throw new Exception("Not all moves were executed");

            _agent.SetState(new TetrisAnalyzeState(_agent, _agent.GameState.Piece.Tetromino));
        }
    }
}
