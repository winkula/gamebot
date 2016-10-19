using System;
using System.Collections.Generic;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using NLog;

namespace GameBot.Game.Tetris.Agents.States
{
    public class TetrisRepeatState : ITetrisAgentState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly TetrisAgent _agent;

        private readonly Move _lastMove;
        private readonly Queue<Move> _pendingMoves;
        private readonly Piece _tracedPiece;

        public TetrisRepeatState(TetrisAgent agent, Move lastMove, Queue<Move> pendingMoves, Piece tracedPiece)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            if (pendingMoves == null) throw new ArgumentNullException(nameof(pendingMoves));
            if (tracedPiece == null) throw new ArgumentNullException(nameof(tracedPiece));

            _agent = agent;

            _lastMove = lastMove;
            _pendingMoves = pendingMoves;
            _tracedPiece = tracedPiece;
        }

        public void Extract()
        {
            // do nothing
        }

        public void Play()
        {
            // repeat the command
            Repeat(_lastMove);

            // check again
            SetStateCheck();
        }

        private void SetStateCheck()
        {
            _agent.SetStateAndContinue(new TetrisCheckState(_agent, _lastMove, _pendingMoves, _tracedPiece));
        }
        
        private void Repeat(Move move)
        {
            _logger.Info($"Repeat command {move}");

            switch (move)
            {
                case Move.Left:
                    _agent.Executor.Hit(Button.Left);
                    break;

                case Move.Right:
                    _agent.Executor.Hit(Button.Right);
                    break;

                case Move.Rotate:
                    // clockwise rotation
                    _agent.Executor.Hit(Button.A); 
                    break;

                case Move.RotateCounterclockwise:
                    // counterclockwise rotation
                    _agent.Executor.Hit(Button.B);
                    break;

                case Move.Drop:
                    throw new ApplicationException("Can't repeat drop");
            }
        }
    }
}
