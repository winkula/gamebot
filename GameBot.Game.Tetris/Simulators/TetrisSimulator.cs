using System;
using System.Collections.Generic;
using System.Linq;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Simulators
{
    public class TetrisSimulator
    {
        public GameState GameState { get; }

        public TetrisSimulator(GameState gameState = null)
        {
            var board = new Board();
            var piece = new Piece();
            var nextPiece = Tetriminos.GetRandom();

            GameState = gameState ?? new GameState(board, piece, nextPiece) { StartLevel = 9 };
        }

        public void Simulate(Move move)
        {
            switch (move)
            {
                case Move.Left: GameState.Left(); break;
                case Move.Right: GameState.Right(); break;
                case Move.Rotate: GameState.Rotate(); break;
                case Move.RotateCounterclockwise: GameState.RotateCounterclockwise(); break;
                case Move.Fall: GameState.Fall(); break;
                case Move.Drop: GameState.Drop(); break;
            }
        }

        public void SimulateRealtime(IList<Move> moves, int msPerAction)
        {
            if (!moves.Any()) return;

            var movesParallel = GetMovesParallel(moves);
            var msTimePassed = 0;
            int fallen = 0;

            foreach (var moveParallel in movesParallel)
            {
                foreach (var move in moveParallel)
                {
                    Simulate(move);
                }

                if (moveParallel.Any(x => x != Move.Drop))
                {
                    msTimePassed += msPerAction;

                    var distance = TetrisLevel.GetFallDistance(GameState.Level, TimeSpan.FromMilliseconds(msTimePassed), GameState.HeartMode);
                    int deltaDistance = (int)Math.Floor(distance) - fallen;
                    if (deltaDistance > 0)
                    {
                        // time passed, fall
                        for (int i = 0; i < deltaDistance; i++)
                        {
                            Simulate(Move.Fall);
                        }

                        fallen += deltaDistance;
                    }
                }
            }
        }

        private ICollection<ICollection<Move>> GetMovesParallel(IEnumerable<Move> moves)
        {
            var movesParallel = new List<ICollection<Move>>();
            var movesList = moves.ToList();

            var rotations = movesList.Where(x => x == Move.Rotate || x == Move.RotateCounterclockwise).ToList();
            var translations = movesList.Where(x => x == Move.Left || x == Move.Right).ToList();

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

        public override string ToString()
        {
            return GameState.ToString();
        }
    }
}
