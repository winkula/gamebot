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
        private readonly TimeSpan _tracedPieceTimestamp;

        private readonly bool _checkCommands;

        public TetrisExecuteState(TetrisAgent agent, Queue<Move> pendingMoves, Piece tracedPiece, TimeSpan tracedPieceTimestamp)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            if (pendingMoves == null) throw new ArgumentNullException(nameof(pendingMoves));
            if (!pendingMoves.Any()) throw new ArgumentException("pendingMoves contains no elements");
            if (tracedPiece == null) throw new ArgumentNullException(nameof(tracedPiece));

            if (agent.GameState == null) throw new ArgumentNullException(nameof(agent.GameState));
            if (agent.GameState.Piece == null) throw new ArgumentNullException(nameof(agent.GameState.Piece));
            if (agent.GameState.NextPiece == null) throw new ArgumentNullException(nameof(agent.GameState.NextPiece));

            _agent = agent;
            _pendingMoves = pendingMoves;
            _tracedPiece = tracedPiece;
            _tracedPieceTimestamp = tracedPieceTimestamp;

            _checkCommands = _agent.Config.Read("Game.Tetris.Check.Enabled", false);
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

                if (_checkCommands)
                {
                    SetStateCheck(move);
                }
                else
                {
                    SetStatePseudoCheck(move);
                }
            }
        }

        private void ExecuteDrop()
        {
            // calculates drop distance, score and new level
            var linesBefore = _agent.GameState.Lines;
            var dropDistance = _agent.GameState.Drop();
            var dropDuration = TetrisTiming.GetDropDuration(dropDistance);
            int linesRemoved = 0;

            if (_agent.GameState.Lines > linesBefore)
            {
                // lines were removed, add extra time
                dropDuration += TetrisTiming.LineRemovingDuration;

                linesRemoved = _agent.GameState.Lines - linesBefore;
            }
            
            // we subtract a time padding, because we dont want to wait the
            // theoretical drop duration, but the real drop duration
            // (we don't want to miss an important frame in analyze state)  
            var waitDuration = dropDuration - Timing.DropDurationPaddingTime;
            if (waitDuration < TimeSpan.Zero)
            {
                waitDuration = TimeSpan.Zero;
            }

            _logger.Info($"Execute Drop (new score {_agent.GameState.Score}, {linesRemoved} lines removed, sleep {waitDuration.Milliseconds} ms)");

            // execute the drop blocking
            // we must wait until the drop is ended before we can continue
            // TODO: here we could do some precalculations for the next search (and execute the drop asynchronous)???
            _agent.Executor.Hold(Button.Down, waitDuration);
        }
        
        private void Execute(Move move)
        {
            _logger.Info($"Execute {move}");
            _agent.TracedPiece = new Piece(_tracedPiece).Apply(move);

            var gameStateSimulation = new GameState(_agent.GameState);
            switch (move)
            {
                case Move.Left:
                    _agent.Executor.Hit(Button.Left);
                    gameStateSimulation.Left();
                    break;

                case Move.Right:
                    _agent.Executor.Hit(Button.Right);
                    gameStateSimulation.Right();
                    break;

                case Move.Rotate:
                    _agent.Executor.Hit(Button.A); // clockwise rotation
                    gameStateSimulation.Rotate();
                    break;

                case Move.RotateCounterclockwise:
                    _agent.Executor.Hit(Button.B); // counterclockwise rotation
                    gameStateSimulation.RotateCounterclockwise();
                    break;

                case Move.Drop:
                    throw new ApplicationException("no drop allowed here!");
            }
        }

        private void SetStateCheck(Move lastMove)
        {
            if (lastMove == Move.Drop) throw new Exception("Can't check drop");

            _agent.SetState(new TetrisCheckState(_agent, lastMove, _pendingMoves, _tracedPiece, _tracedPieceTimestamp));
        }

        private void SetStatePseudoCheck(Move lastMove)
        {
            if (lastMove == Move.Drop) throw new Exception("Can't check drop");

            _agent.SetStateAndContinue(new TetrisPseudoCheckState(_agent, lastMove, _pendingMoves, _tracedPiece, _tracedPieceTimestamp));
        }

        private void SetStateAnalyze()
        {
            if (_pendingMoves.Any()) throw new Exception("Not all moves were executed");

            _agent.SetState(new TetrisAnalyzeState(_agent, _agent.GameState.Piece.Tetrimino));
        }
    }
}
