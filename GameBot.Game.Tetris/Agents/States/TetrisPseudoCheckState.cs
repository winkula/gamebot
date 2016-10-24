using System;
using System.Collections.Generic;
using GameBot.Game.Tetris.Data;
using NLog;

namespace GameBot.Game.Tetris.Agents.States
{
    public class TetrisPseudoCheckState : ITetrisAgentState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly TetrisAgent _agent;

        private readonly Move _lastMove;
        private readonly Queue<Move> _pendingMoves;

        private Piece _tracedPiece;
        private TimeSpan _tracedPieceTimestamp;

        public TetrisPseudoCheckState(TetrisAgent agent, Move lastMove, Queue<Move> pendingMoves, Piece tracedPiece, TimeSpan tracedPieceTimestamp)
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
            var screenshot = _agent.Screenshot;
            var pieceMoved = new Piece(_tracedPiece).Apply(_lastMove);
            var timestamp = screenshot.Timestamp;
            
            Success(pieceMoved, timestamp);                        
        }
        
        private void Success(Piece newPosition, TimeSpan now)
        {
            _logger.Info($"Execution successful ({_lastMove})");

            // move was successfully executed
            // we remove it from the queue
            UpdateCurrentPiece(newPosition, now);

            // command was executed successfully
            // we go now to the next command
            SetStateExecute();
        }
        
        private void UpdateCurrentPiece(Piece tracedPieceNew, TimeSpan tracedPiecetTimestampNew)
        {
            _tracedPiece = tracedPieceNew;
            _agent.TracedPiece = tracedPieceNew;

            _agent.GameState.Piece = new Piece(_tracedPiece);
            _tracedPieceTimestamp = tracedPiecetTimestampNew;
        }
        
        private void SetStateExecute()
        {
            _agent.SetStateAndContinue(new TetrisExecuteState(_agent, _pendingMoves, _tracedPiece, _tracedPieceTimestamp));
        }
    }
}
