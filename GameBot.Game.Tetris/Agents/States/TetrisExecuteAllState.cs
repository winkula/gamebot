using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris.Agents.States
{
    public class TetrisExecuteAllState : ITetrisAgentState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly TetrisAgent _agent;
        
        private readonly ICollection<ICollection<Move>> _pendingMoves;
        private readonly Piece _tracedPiece;
        
        public TetrisExecuteAllState(TetrisAgent agent, IList<Move> pendingMoves, Piece tracedPiece)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            if (pendingMoves == null) throw new ArgumentNullException(nameof(pendingMoves));
            if (!pendingMoves.Any()) throw new ArgumentException("pendingMoves contains no elements");
            if (tracedPiece == null) throw new ArgumentNullException(nameof(tracedPiece));

            if (agent.GameState == null) throw new ArgumentNullException(nameof(agent.GameState));
            if (agent.GameState.Piece == null) throw new ArgumentNullException(nameof(agent.GameState.Piece));
            if (agent.GameState.NextPiece == null) throw new ArgumentNullException(nameof(agent.GameState.NextPiece));

            _agent = agent;

            _pendingMoves = GetMovesParallel(pendingMoves);
            _tracedPiece = new Piece(tracedPiece);
        }

        private ICollection<ICollection<Move>> GetMovesParallel(IList<Move> moves)
        {
            var movesParallel = new List<ICollection<Move>>();

            var rotations = moves.Where(x => x == Move.Rotate || x == Move.RotateCounterclockwise).ToList();
            var translations = moves.Where(x => x == Move.Left || x == Move.Right).ToList();

            for (int i = 0; i < Math.Max(rotations.Count, translations.Count); i++)
            {
                var movesCombined = new List<Move>();

                if (rotations.Count > i)
                {
                    movesCombined.Add(rotations[i]);
                }
                if (translations.Count > i)
                {
                    movesCombined.Add(translations[i]);
                }

                movesParallel.Add(movesCombined);
            }

            movesParallel.Add(new List<Move> {  Move.Drop });
            return movesParallel;
        }

        public void Extract()
        {
            // do nothing
        }

        public void Play()
        {
            foreach (var parallelMoves in _pendingMoves)
            {
                // next move to execute
                if (!parallelMoves.Any()) throw new Exception("Must contain any moves");

                if (parallelMoves.All(x => x == Move.Drop))
                {
                    ExecuteDrop();
                    SetStateAnalyze();
                    return;
                }

                Execute(parallelMoves);
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
                //var lineRemoveDuration = TetrisTiming.LineRemovingDuration();
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
            _agent.Executor.Hit(Button.Down, waitDuration);
        }
        
        private void Execute(ICollection<Move> moves)
        {
            _logger.Info($"Execute {string.Join(", ", moves)}");
            
            _agent.Executor.Hit(moves.Select(x => x.ToButton()));
            
            var gameStateSimulation = new GameState(_agent.GameState);

            foreach (var move in moves)
            {
                if (move == Move.Drop) throw new Exception("Drop not allowed here");

                _tracedPiece.Apply(move);
                move.Apply(gameStateSimulation);
            }
            
            UpdateCurrentPiece(_tracedPiece);
        }

        private void UpdateCurrentPiece(Piece newPiece)
        {
            _agent.TracedPiece = newPiece;
            _agent.ExpectedPiece = newPiece;
            _agent.GameState.Piece = new Piece(newPiece);
        }

        private void SetStateAnalyze()
        {
            _agent.SetState(new TetrisAnalyzeState(_agent, _agent.GameState.Piece.Tetrimino));
        }
    }
}
