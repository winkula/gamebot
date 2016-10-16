using System;
using System.Collections.Generic;

namespace GameBot.Game.Tetris.Data
{
    public enum Tetromino
    {
        O = 0,
        I = 1,
        S = 2,
        Z = 3,
        L = 4,
        J = 5,
        T = 6
    }

    public static class Tetrominos
    {
        private static readonly Random Random = new Random();

        // Chances found by sampling dozens of emulator tetris game states
        private static readonly double[] Chances = { 0.149, 0.130, 0.196, 0.100, 0.113, 0.145, 0.167 };

        public static readonly Tetromino[] All = { Tetromino.O, Tetromino.I, Tetromino.S, Tetromino.Z, Tetromino.L, Tetromino.J, Tetromino.T };

        public static Tetromino GetRandom()
        {
            var p = Random.NextDouble();
            if (p < 0.149) return Tetromino.O;
            if (p < 0.279) return Tetromino.I;
            if (p < 0.475) return Tetromino.S;
            if (p < 0.575) return Tetromino.Z;
            if (p < 0.688) return Tetromino.L;
            if (p < 0.833) return Tetromino.J;
            return Tetromino.T;
        }

        public static double GetChance(Tetromino tetromino)
        {
            return Chances[(int)tetromino];
        }
    }

    public static class TetrominoExtensions
    {
        public static double GetChance(this Tetromino tetromino)
        {
            return Tetrominos.GetChance(tetromino);
        }

        public static IEnumerable<int> GetPossibleOrientations(this Tetromino tetromino)
        {
            // TODO: lookup table
            switch (tetromino)
            {
                case Tetromino.O:
                    return new int[] { 0 };
                case Tetromino.I:
                case Tetromino.S:
                case Tetromino.Z:
                    return new[] { 0, 1 };
                case Tetromino.L:
                case Tetromino.J:
                case Tetromino.T:
                    return new int[] { 0, 1, 2, 3 };
                default:
                    throw new ArgumentException("illegal tetromino");
            };
        }
    }
}
