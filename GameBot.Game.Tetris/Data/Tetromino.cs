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
        public const int AllPossibleOrientations = 19;

        /// <summary>
        /// All possible Tetrominos.
        /// </summary>
        public static readonly Tetromino[] All = { Tetromino.O, Tetromino.I, Tetromino.S, Tetromino.Z, Tetromino.L, Tetromino.J, Tetromino.T };

        private static readonly Random _random = new Random();

        // Chances found by sampling dozens of emulator tetris game states
        private static readonly double[] _chances = { 0.149, 0.130, 0.196, 0.100, 0.113, 0.145, 0.167 };
        private static readonly int[][] _orientations = { new[] { 0 }, new[] { 0, 1 }, new[] { 0, 1 }, new[] { 0, 1 }, new[] { 0, 1, 2, 3 }, new[] { 0, 1, 2, 3 }, new[] { 0, 1, 2, 3 } };

        public static Tetromino GetRandom()
        {
            var p = _random.NextDouble();
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
            return _chances[(int)tetromino];
        }

        public static IEnumerable<int> GetPossibleOrientations(Tetromino tetromino)
        {
            return _orientations[(int)tetromino];
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
            return Tetrominos.GetPossibleOrientations(tetromino);
        }
        
        // gets all start configurations that are possible for a specified tetromino
        public static IEnumerable<Piece> GetPoses(this Tetromino tetromino)
        {
            Shape shape;

            // TODO: make static lookup table
            switch (tetromino)
            {
                case Tetromino.O:
                    shape = Shape.Get(tetromino);
                    for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                        yield return new Piece(tetromino, 0, translation);
                    break;
                case Tetromino.I:
                    foreach (int rotation in new[] { 0, 1 })
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Piece(tetromino, rotation, translation);
                    }
                    break;
                case Tetromino.S:
                    foreach (int rotation in new[] { 0, 1 })
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Piece(tetromino, rotation, translation);
                    }
                    break;
                case Tetromino.Z:
                    foreach (int rotation in new[] { 0, 1 })
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Piece(tetromino, rotation, translation);
                    }
                    break;
                case Tetromino.L:
                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Piece(tetromino, rotation, translation);
                    }
                    break;
                case Tetromino.J:
                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Piece(tetromino, rotation, translation);
                    }
                    break;
                case Tetromino.T:
                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Piece(tetromino, rotation, translation);
                    }
                    break;
            }
        }
    }
}
