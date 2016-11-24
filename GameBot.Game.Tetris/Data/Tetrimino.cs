using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris.Data
{
    public enum Tetrimino
    {
        O = 0,
        I = 1,
        S = 2,
        Z = 3,
        L = 4,
        J = 5,
        T = 6
    }

    public static class Tetriminos
    {
        public const int AllPossibleOrientations = 19;

        /// <summary>
        /// All possible Tetriminos.
        /// </summary>
        public static readonly Tetrimino[] All = { Tetrimino.O, Tetrimino.I, Tetrimino.S, Tetrimino.Z, Tetrimino.L, Tetrimino.J, Tetrimino.T };

        private static readonly Random _random = new Random();

        // Chances found by sampling dozens of emulator tetris game states
        private static readonly double[] _chances = { 0.149, 0.130, 0.196, 0.100, 0.113, 0.145, 0.167 };

        // ordered, so that the bot has to press as little buttons as possible
        private static readonly int[][] _orientations = { new[] { 0 }, new[] { 0, 1 }, new[] { 0, 1 }, new[] { 0, 1 }, new[] { 0, 1, 3, 2 }, new[] { 0, 1, 3, 2 }, new[] { 0, 1, 3, 2 } };

        public static Tetrimino GetRandom(Random random = null)
        {
            var p = (random ?? _random).NextDouble();
            if (p < 0.149) return Tetrimino.O;
            if (p < 0.279) return Tetrimino.I;
            if (p < 0.475) return Tetrimino.S;
            if (p < 0.575) return Tetrimino.Z;
            if (p < 0.688) return Tetrimino.L;
            if (p < 0.833) return Tetrimino.J;
            return Tetrimino.T;
        }

        public static double GetChance(Tetrimino tetrimino)
        {
            return _chances[(int)tetrimino];
        }

        public static IEnumerable<int> GetPossibleOrientations(Tetrimino tetrimino)
        {
            return _orientations[(int)tetrimino];
        }

        public static IEnumerable<int> GetPossibleTranslations(Tetrimino tetrimino, int orientation = 0)
        {
            var shape = Shape.Get(tetrimino, orientation);
            for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
            {
                yield return translation;
            }
        }
    }

    public static class TetrominoExtensions
    {
        public static double GetChance(this Tetrimino tetrimino)
        {
            return Tetriminos.GetChance(tetrimino);
        }

        public static IEnumerable<int> GetPossibleOrientations(this Tetrimino tetrimino)
        {
            return Tetriminos.GetPossibleOrientations(tetrimino);
        }

        public static IEnumerable<int> GetPossibleTranslations(this Tetrimino tetrimino, int orientation = 0)
        {
            return Tetriminos.GetPossibleTranslations(tetrimino, orientation);
        }

        // gets all start configurations that are possible for a specified tetrimino
        public static IEnumerable<Piece> GetPoses(this Tetrimino tetrimino)
        {
            return TetriminoLookups.Instance
                .GetPoses(tetrimino)
                .Select(x => new Piece(tetrimino, x.Orientation, x.Translation));
        }
    }
}
