﻿using System.Collections.Generic;

namespace GameBot.Game.Tetris.Data
{
    public class Way
    {
        public int Rotation { get; }
        public int Translation { get; }
        public int Fall { get; }

        public Way(int rotation, int translation, int fall = 0)
        {
            Rotation = rotation;
            Translation = translation;
            Fall = fall;
        }

        // TODO: make separate class for start settings?
        public static IEnumerable<Way> GetAll(Tetromino tetromino)
        {
            Shape shape;

            // TODO: make static lookup table
            switch (tetromino)
            {
                case Tetromino.O:
                    shape = Shape.Get(tetromino);
                    for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                        yield return new Way(0, translation);
                    break;
                case Tetromino.I:
                    foreach (int rotation in new[] { 0, 1 })
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Way(rotation, translation);
                    }
                    break;
                case Tetromino.S:
                    foreach (int rotation in new[] { 0, 1 })
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Way(rotation, translation);
                    }
                    break;
                case Tetromino.Z:
                    foreach (int rotation in new[] { 0, 1 })
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Way(rotation, translation);
                    }
                    break;
                case Tetromino.L:
                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Way(rotation, translation);
                    }
                    break;
                case Tetromino.J:
                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Way(rotation, translation);
                    }
                    break;
                case Tetromino.T:
                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Way(rotation, translation);
                    }
                    break;
            }
        }

        public override string ToString()
        {
            return $"Move {{ Rotation: {Rotation}, Translation: {Translation}, Fall: {Fall} }}";
        }
    }
}
