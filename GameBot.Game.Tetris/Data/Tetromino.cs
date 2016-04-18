using System;

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

    public class Tetrominos
    {
        private static readonly Random random = new Random();

        // Chances found by sampling dozens of emulator tetris game states
        private static readonly double[] chances = new double[] { 0.149, 0.130, 0.196, 0.100, 0.113, 0.145, 0.167 };

        public static Tetromino GetRandom()
        {
            var p = random.NextDouble();
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
            return chances[(int)tetromino];
        }
    }

    public static class TetrominoExtensions
    {
        public static double GetChance(this Tetromino tetromino)
        {
            return Tetrominos.GetChance(tetromino);
        }
    }
}
