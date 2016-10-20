using System;
using System.Collections.Generic;

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
        private static readonly int[][] _orientations = { new[] { 0 }, new[] { 0, 1 }, new[] { 0, 1 }, new[] { 0, 1 }, new[] { 0, 1, 2, 3 }, new[] { 0, 1, 2, 3 }, new[] { 0, 1, 2, 3 } };

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
        
        // gets all start configurations that are possible for a specified tetrimino
        public static IEnumerable<Piece> GetPoses(this Tetrimino tetrimino)
        {
            Shape shape;

            // TODO: make static lookup table
            switch (tetrimino)
            {
                case Tetrimino.O:
                    shape = Shape.Get(tetrimino);
                    for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                        yield return new Piece(tetrimino, 0, translation);
                    break;
                case Tetrimino.I:
                    foreach (int rotation in new[] { 0, 1 })
                    {
                        shape = Shape.Get(tetrimino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Piece(tetrimino, rotation, translation);
                    }
                    break;
                case Tetrimino.S:
                    foreach (int rotation in new[] { 0, 1 })
                    {
                        shape = Shape.Get(tetrimino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Piece(tetrimino, rotation, translation);
                    }
                    break;
                case Tetrimino.Z:
                    foreach (int rotation in new[] { 0, 1 })
                    {
                        shape = Shape.Get(tetrimino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Piece(tetrimino, rotation, translation);
                    }
                    break;
                case Tetrimino.L:
                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        shape = Shape.Get(tetrimino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Piece(tetrimino, rotation, translation);
                    }
                    break;
                case Tetrimino.J:
                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        shape = Shape.Get(tetrimino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Piece(tetrimino, rotation, translation);
                    }
                    break;
                case Tetrimino.T:
                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        shape = Shape.Get(tetrimino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Piece(tetrimino, rotation, translation);
                    }
                    break;
            }
        }
    }
}
