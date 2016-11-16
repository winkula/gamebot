using System;
using System.Collections.Generic;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Extraction.Samplers;
using NLog;

namespace GameBot.Game.Tetris.Agents.States
{
    public class TetrisCheckState : ITetrisAgentState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly TetrisAgent _agent;

        private readonly ISampler<bool> _moveConfirmationSampler;

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
            _moveConfirmationSampler = new MoveConfirmationSampler(_agent.CheckSamples);
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
            var searchHeight = CalulateSearchHeight();
            _logger.Info($"Check {_lastMove} (search height {searchHeight})");

            // extract
            bool pieceHasMoved;
            var screenshot = _agent.Screenshot;
            var extractedCurrentPiece = _agent.Extractor.ExtractMovedPiece(screenshot, new Piece(_tracedPiece), _lastMove, searchHeight, out pieceHasMoved);
            
            if (extractedCurrentPiece == null)
            {
                // both hypotheses rejected: piece not found on the screenshot
                // no problem, we get a new screenshot and try it again ;)
                // TODO: maybe the piece is not visble at all? handle pause menu and rocket cutscenes
                PieceNotFound(_tracedPiece.Tetrimino);
            }
            else
            {
                var timestamp = screenshot.Timestamp;
                
                // add sample
                var samplePseudoProbability = 0.5; // TODO: remove dummy value?
                _moveConfirmationSampler.Sample(new ProbabilisticResult<bool>(pieceHasMoved, samplePseudoProbability));
                _logger.Info($"Add sample to decide movement ({_lastMove})");

                // enought samples?
                if (_moveConfirmationSampler.IsComplete)
                {
                    if (_moveConfirmationSampler.Result)
                    {
                        Success(extractedCurrentPiece, timestamp);
                    }
                    else
                    {
                        Fail(extractedCurrentPiece, timestamp);
                    }
                }
            }
        }

        private void PieceNotFound(Tetrimino tetrimino)
        {
            _logger.Warn($"Piece not recognized ({tetrimino})");
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

        private void Fail(Piece newPosition, TimeSpan now)
        {
            _logger.Warn($"Execution failed ({_lastMove})");

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

            var searchHeightTime = TetrisLevel.GetFallDistance(_agent.GameState.Level, duration);
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
