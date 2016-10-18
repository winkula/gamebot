﻿using GameBot.Core.Data;
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

        private readonly Queue<Move> _moves;

        private Move? _lastMove;
        private Piece _lastPosition;
        private TimeSpan _lastPositionTimeStamp;
        
        public TetrisExecuteState(TetrisAgent agent, Queue<Move> moves, TimeSpan analyzeTimestamp)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            if (moves == null) throw new ArgumentNullException(nameof(moves));
            if (analyzeTimestamp == null) throw new ArgumentNullException(nameof(analyzeTimestamp));

            _agent = agent;

            _moves = moves;
            _lastPositionTimeStamp = analyzeTimestamp;

            if (agent.GameState == null) throw new ArgumentNullException(nameof(agent.GameState));
            if (agent.GameState.Piece == null) throw new ArgumentNullException(nameof(agent.GameState.Piece));
            if (agent.GameState.NextPiece == null) throw new ArgumentNullException(nameof(agent.GameState.NextPiece));

            _lastPosition = new Piece(agent.GameState.Piece);
        }

        public void Act()
        {
            // first we have to check if the last command was successful
            if (_lastMove != null)
            {
                var now = _agent.Clock.Time;
                var expectedFallDistance = GetExpectedFallDistance(now);
                _logger.Info("Check command");
                _logger.Info($"Search piece with search height {expectedFallDistance}");

                var pieceNotMoved = _lastPosition;
                var pieceMoved = new Piece(_lastPosition).Apply(_lastMove.Value);

                var resultPieceNotMoved = _agent.PieceExtractor.ExtractKnownPieceFuzzy(_agent.Screenshot, pieceNotMoved,expectedFallDistance, _agent.ProbabilityThreshold);
                var resultPieceMoved = _agent.PieceExtractor.ExtractKnownPieceFuzzy(_agent.Screenshot, pieceMoved,expectedFallDistance, _agent.ProbabilityThreshold);

                if (resultPieceNotMoved.Result == null && resultPieceMoved.Result == null)
                {
                    // piece not found
                    PieceNotFound(_lastPosition.Tetromino);
                    return;
                }
                
                if (resultPieceNotMoved.Result == null || resultPieceMoved.Probability >= resultPieceNotMoved.Probability)
                {
                    _logger.Info("Command executed");

                    // move was successfully executed
                    // we remove it from the queue
                    UpdateLastPosition(resultPieceMoved.Result, now);
                    ProceedToNextCommand();
                }
                else if (resultPieceMoved.Result == null || resultPieceNotMoved.Probability >= resultPieceMoved.Probability)
                {
                    _logger.Warn("Command failed");

                    // the command was not executed and the tile is in the old position
                    UpdateLastPosition(resultPieceNotMoved.Result, now);
                    RepeatCommand();
                    return; // we return here because we need a new screenshot
                }
            }

            // are there commands to execute?
            if (_moves.Any())
            {
                var move = _moves.Peek();

                // first execution...

                if (move != Move.Drop)
                {
                    ExecuteAndCheck(move);
                }
                else if (move == Move.Drop && _moves.Count == 1)
                {
                    // cleanup (not realy necessary, because we leave the state anyway)
                    _lastMove = null;
                    ProceedToNextCommand();

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
                else
                {
                    throw new Exception("Drop must be the last move to execute.");
                }

                if (!_moves.Any() && !_lastMove.HasValue)
                {
                    // we executed all moves and have no comannd to check
                    // back to the analyze state
                    SetStateAnalyze();
                }
            }
        }

        private void PieceNotFound(Tetromino tetromino)
        {
            _logger.Warn($"Piece not recognized ({tetromino})");
        }

        private int GetExpectedFallDistance(TimeSpan now)
        {
            // we add some time to the theoretical duration between now and the
            // timestamp of the last analyzed screenshot
            // so we are sure, that we don't miss the piece
            var duration = 
                (now - _lastPositionTimeStamp) + 
                TimeSpan.FromMilliseconds(Timing.ExpectedFallDurationPadding);

            return TetrisLevel.GetMaxFallDistance(_agent.GameState.Level, duration);
        }

        private void UpdateLastPosition(Piece newLastPosition, TimeSpan newLastPositionTimestamp)
        {
            _lastPosition = newLastPosition;
            _agent.GameState.Piece = new Piece(_lastPosition);
            _lastPositionTimeStamp = newLastPositionTimestamp;
        }

        private void ProceedToNextCommand()
        {
            _moves.Dequeue();
        }

        private void RepeatCommand()
        {
            if (!_lastMove.HasValue)
                throw new ArgumentNullException(nameof(_lastMove));

            // repeated execution
            ExecuteAndCheck(_lastMove.Value);
        }

        private void ExecuteAndCheck(Move move)
        {
            _lastMove = move;

            Execute(move);
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

        private void SetStateAnalyze()
        {
            _agent.SetState(new TetrisAnalyzeState(_agent, _agent.GameState.Piece.Tetromino));
        }
    }
}
