using System;
using System.Collections.Generic;
using GameBot.Game.Tetris.Data;
using NLog;

namespace GameBot.Game.Tetris.Agents.States
{
    public class TetrisCheckState : ITetrisAgentState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly TetrisAgent _agent;

        private readonly Move _lastMove;
        private readonly Queue<Move> _pendingMoves;

        private Piece _tracedPiece;
        private TimeSpan _lastPositionTimeStamp;

        public TetrisCheckState(TetrisAgent agent, Move lastMove, Queue<Move> pendingMoves, Piece tracedPiece)
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
            _logger.Info($"Check command {_lastMove}");

            var now = _agent.Clock.Time;
            var expectedFallDistance = GetExpectedFallDistance(now);
            _logger.Info($"Search piece with search height {expectedFallDistance}");

            var pieceNotMoved = _tracedPiece;
            var pieceMoved = new Piece(_tracedPiece).Apply(_lastMove);

            // extract
            var resultPieceNotMoved = _agent.PieceExtractor.ExtractKnownPieceFuzzy(_agent.Screenshot, pieceNotMoved, expectedFallDistance, _agent.ProbabilityThreshold);
            var resultPieceMoved = _agent.PieceExtractor.ExtractKnownPieceFuzzy(_agent.Screenshot, pieceMoved, expectedFallDistance, _agent.ProbabilityThreshold);

            if (resultPieceNotMoved.Result == null && resultPieceMoved.Result == null)
            {
                // piece not found
                // no problem, we get a new screenshot and try it again ;)
                PieceNotFound(_tracedPiece.Tetromino);
            }
            else
            {
                if (resultPieceNotMoved.Result == null || resultPieceMoved.Probability >= resultPieceNotMoved.Probability)
                {
                    Success(resultPieceMoved.Result, now);
                }
                else if (resultPieceMoved.Result == null || resultPieceNotMoved.Probability >= resultPieceMoved.Probability)
                {
                    Fail(resultPieceNotMoved.Result, now);
                }
            }
        }

        private void PieceNotFound(Tetromino tetromino)
        {
            _logger.Warn($"Piece not recognized ({tetromino})");
        }

        private void Success(Piece newPosition, TimeSpan now)
        {
            _logger.Info("Command executed");

            // move was successfully executed
            // we remove it from the queue
            UpdateLastPosition(newPosition, now);
            
            // command was executed successfully
            // we go now to the next command
            SetStateExecute();
        }

        private void Fail(Piece newPosition, TimeSpan now)
        {
            _logger.Warn("Command failed");

            // the command was not executed and the tile is in the old position
            UpdateLastPosition(newPosition, now);
            
            // repeat the command
            SetStateRepeat();
        }
        
        private int GetExpectedFallDistance(TimeSpan now)
        {
            // we add some time to the theoretical duration between now and the
            // timestamp of the last analyzed screenshot
            // so we are sure, that we don't miss the piece
            var duration =
                (now - _lastPositionTimeStamp) +
                TimeSpan.FromMilliseconds(Timing.ExpectedFallDurationPadding);

            var searchHeightTime = TetrisLevel.GetMaxFallDistance(_agent.GameState.Level, duration);
            var searchHeightMax = _agent.GameState.Board.DropDistance(_tracedPiece);

            return Math.Min(searchHeightTime, searchHeightMax);
        }

        private void UpdateLastPosition(Piece newLastPosition, TimeSpan newLastPositionTimestamp)
        {
            _tracedPiece = newLastPosition;
            _agent.GameState.Piece = new Piece(_tracedPiece);
            _lastPositionTimeStamp = newLastPositionTimestamp;
        }
        
        private void SetStateRepeat()
        {
            _agent.SetStateAndContinue(new TetrisRepeatState(_agent, _lastMove, _pendingMoves, _tracedPiece));
        }

        private void SetStateExecute()
        {
            // TODO: do we need this timestamp??
            _agent.SetStateAndContinue(new TetrisExecuteState(_agent, _pendingMoves, _tracedPiece/*, _lastPositionTimeStamp*/));
        }
    }
}
