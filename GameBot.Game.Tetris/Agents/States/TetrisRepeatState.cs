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
        private readonly TimeSpan _tracedPieceTimestamp;

        public TetrisRepeatState(TetrisAgent agent, Move lastMove, Queue<Move> pendingMoves, Piece tracedPiece, TimeSpan tracedPieceTimestamp)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            if (pendingMoves == null) throw new ArgumentNullException(nameof(pendingMoves));
            if (tracedPiece == null) throw new ArgumentNullException(nameof(tracedPiece));

            _agent = agent;
            _lastMove = lastMove;
            _pendingMoves = pendingMoves;
            _tracedPiece = tracedPiece;
            _tracedPieceTimestamp = tracedPieceTimestamp;
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
            _agent.SetState(new TetrisCheckState(_agent, _lastMove, _pendingMoves, _tracedPiece, _tracedPieceTimestamp));
        }
        
        private void Repeat(Move move)
        {
            _logger.Info($"Repeat command {move}");
            _agent.ExpectedPiece = new Piece(_tracedPiece).Apply(move);

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
                    // clockwise rotation
                    _agent.Executor.Hit(Button.A);
                    gameStateSimulation.Rotate();
                    break;

                case Move.RotateCounterclockwise:
                    // counterclockwise rotation
                    _agent.Executor.Hit(Button.B);
                    gameStateSimulation.RotateCounterclockwise();
                    break;

                case Move.Drop:
                    throw new ApplicationException("Can't repeat drop");
            }
        }
    }
}
