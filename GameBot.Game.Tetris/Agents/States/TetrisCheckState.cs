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
        private TimeSpan _tracedPieceTimestamp;

        public TetrisCheckState(TetrisAgent agent, Move lastMove, Queue<Move> pendingMoves, Piece tracedPiece, TimeSpan tracedPieceTimestamp)
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
            _logger.Info($"Check command {_lastMove}");
            
            var searchHeight = CalulateSearchHeight();
            _logger.Info($"Search height for check is {searchHeight}");

            var pieceNotMoved = _tracedPiece;
            var pieceMoved = new Piece(_tracedPiece).Apply(_lastMove);

            // extract
            var screenshot = _agent.Screenshot;
            var resultPieceNotMoved = _agent.PieceExtractor.ExtractKnownPieceFuzzy(screenshot, pieceNotMoved, searchHeight);
            var resultPieceMoved = _agent.PieceExtractor.ExtractKnownPieceFuzzy(screenshot, pieceMoved, searchHeight);

            var lowerThreshold = _agent.ExtractionLowerThreshold;
            if (resultPieceNotMoved.IsRejected(lowerThreshold) && resultPieceMoved.IsRejected(lowerThreshold))
            {
                // piece not found
                // no problem, we get a new screenshot and try it again ;)
                PieceNotFound(_tracedPiece.Tetromino);
            }
            else
            {
                if (resultPieceMoved.Probability >= resultPieceNotMoved.Probability)
                {
                    // its more probable, that the piece moved
                    if (resultPieceMoved.IsAccepted(lowerThreshold))
                    {
                        var timestamp = screenshot.Timestamp;
                        Success(resultPieceMoved.Result, timestamp);
                    }
                    else
                    {
                        PositionUnclear(_tracedPiece.Tetromino);
                    }
                }
                else
                {
                    if (resultPieceNotMoved.IsAccepted(lowerThreshold))
                    {
                        // its more probable, that the piece did not move
                        var timestamp = screenshot.Timestamp;
                        Fail(resultPieceNotMoved.Result, timestamp);
                    }
                    else
                    {
                        PositionUnclear(_tracedPiece.Tetromino);
                    }
                }
            }
        }

        private void PieceNotFound(Tetromino tetromino)
        {
            _logger.Warn($"Piece not recognized ({tetromino})");
        }

        private void PositionUnclear(Tetromino tetromino)
        {
            _logger.Warn($"Not clear, if piece moved or not ({tetromino})");
        }

        private void Success(Piece newPosition, TimeSpan now)
        {
            _logger.Info("Command executed");

            // move was successfully executed
            // we remove it from the queue
            UpdateCurrentPiece(newPosition, now);

            // command was executed successfully
            // we go now to the next command
            SetStateExecute();
        }

        private void Fail(Piece newPosition, TimeSpan now)
        {
            _logger.Warn("Command failed");

            // the command was not executed and the tile is in the old position
            UpdateCurrentPiece(newPosition, now);

            // repeat the command
            SetStateRepeat();
        }

        private int CalulateSearchHeight()
        {
            // we add some time to the theoretical duration between now and the
            // timestamp of the last analyzed screenshot
            // so we are sure, that we don't miss the piece
            var duration = _agent.Screenshot.Timestamp
                - _tracedPieceTimestamp
                + Timing.CheckFallDurationPaddingTime;

            var searchHeightTime = TetrisLevel.GetMaxFallDistance(_agent.GameState.Level, duration);
            var searchHeightMax = _agent.GameState.Board.DropDistance(_tracedPiece);

            return Math.Min(searchHeightTime, searchHeightMax);
        }

        private void UpdateCurrentPiece(Piece tracedPieceNew, TimeSpan tracedPiecetTimestampNew)
        {
            _tracedPiece = tracedPieceNew;
            _agent.TracedPiece = tracedPieceNew;
            
            _agent.GameState.Piece = new Piece(_tracedPiece);
            _tracedPieceTimestamp = tracedPiecetTimestampNew;
        }

        private void SetStateRepeat()
        {
            _agent.SetStateAndContinue(new TetrisRepeatState(_agent, _lastMove, _pendingMoves, _tracedPiece, _tracedPieceTimestamp));
        }

        private void SetStateExecute()
        {
            _agent.SetStateAndContinue(new TetrisExecuteState(_agent, _pendingMoves, _tracedPiece, _tracedPieceTimestamp));
        }
    }
}
