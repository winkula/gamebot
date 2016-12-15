using System;
using System.Collections.Generic;
using System.Linq;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using NLog;

namespace GameBot.Game.Tetris.States
{
    public class ExecuteState : BaseState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private readonly ICollection<ICollection<Move>> _pendingMoves;
        private readonly Piece _tracedPiece;

        public ExecuteState(TetrisAgent agent, IList<Move> pendingMoves, Piece tracedPiece) : base(agent)
        {
            if (pendingMoves == null) throw new ArgumentNullException(nameof(pendingMoves));
            if (!pendingMoves.Any()) throw new ArgumentException("pendingMoves contains no elements");
            if (tracedPiece == null) throw new ArgumentNullException(nameof(tracedPiece));

            if (agent.GameState == null) throw new ArgumentNullException(nameof(agent.GameState));
            if (agent.GameState.Piece == null) throw new ArgumentNullException(nameof(agent.GameState.Piece));
            if (agent.GameState.NextPiece == null) throw new ArgumentNullException(nameof(agent.GameState.NextPiece));
            
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

            movesParallel.Add(new List<Move> { Move.Drop });
            return movesParallel;
        }

        public override void Extract()
        {
            // do nothing
        }

        public override void Play()
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
            // when we were executing button presses, the piece has fallen some rows
            // this is especially relevant in higher levels when speed is higher
            // we let the piece fall
            var alreadyPastTime = Agent.GetExecutionDuration(_pendingMoves.Count) + Agent.LessFallTimeBeforeDrop;
            var alreadyFallenDistance = TetrisLevel.GetFallDistance(Agent.GameState.Level, alreadyPastTime, Agent.GameState.HeartMode);
            
            // calculates drop distance, score and new level
            var linesBefore = Agent.GameState.Lines;
            int linesRemoved = 0;
            // we add one because the current line needs also to be dropped
            var dropDistance = Math.Max(1.0 + Agent.GameState.Drop() - alreadyFallenDistance, 0.0);
            var dropDuration = TetrisTiming.GetDropDuration(dropDistance);
            var waitDuration = TetrisTiming.EntryDelayDuration;

            if (Agent.GameState.Lines > linesBefore)
            {
                // lines were removed
                linesRemoved = Agent.GameState.Lines - linesBefore;

                // additinally wait the line clear duration
                waitDuration += TetrisTiming.LineClearDuration;
            }

            _logger.Info($"Execute Drop (new score {Agent.GameState.Score}, {linesRemoved} lines removed, drop lasts {dropDuration.Milliseconds} ms)");

            // execute the drop blocking
            // we must wait until the drop is ended before we can continue
            // TODO: here we could do some precalculations for the next search (and execute the drop asynchronous)???
            Agent.Executor.Hold(Button.Down, dropDuration);
            _logger.Info("Drop executed");

            // sleep and wait until the lines are cleared
            WaitDrop(waitDuration);
        }

        private void WaitDrop(TimeSpan duration)
        {
            // we subtract a time padding, because we dont want to wait the
            // theoretical drop duration, but the real drop duration
            // (we don't want to miss an important frame in analyze state)  
            var waitDuration = duration - Agent.LessWaitTimeAfterDrop;
            if (waitDuration > TimeSpan.Zero)
            {
                Agent.Clock.Sleep(waitDuration);
            }
        }

        private void Execute(ICollection<Move> moves)
        {
            _logger.Info($"Execute {string.Join(", ", moves)}");

            Agent.Executor.Hit(moves.Select(x => x.ToButton()));

            var gameStateSimulation = new GameState(Agent.GameState);

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
            Agent.TracedPiece = newPiece;
            Agent.GameState.Piece = new Piece(newPiece);
        }

        private void SetStateAnalyze()
        {
            SetState(new AnalyzeState(Agent, Agent.Clock.Time, Agent.GameState.Piece.Tetrimino));
        }
    }
}
