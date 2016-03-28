using System.Collections.Generic;

namespace GameBot.Game.Tetris.Data
{
    public class Move
    {
        public int Rotation { get; }
        public int Translation { get; }
        public int Fall { get; }

        public Move(int rotation, int translation, int fall)
        {
            Rotation = rotation;
            Translation = translation;
            Fall = fall;
        }

        public Move(int rotation, int translation) : this(rotation, translation, 0)
        {
        }

        // TODO: make separate class for start settings?
        public static IEnumerable<Move> GetAllSettings(Tetromino tetromino)
        {
            Shape shape;
            // TODO: make static lookup table
            switch (tetromino)
            {
                case Tetromino.O:
                    shape = Shape.Get(tetromino, 0);
                    for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                        yield return new Move(0, translation);
                    break;
                case Tetromino.I:
                    foreach (int rotation in new[] { 0, 1 })
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Move(rotation, translation);
                    }
                    break;
                case Tetromino.S:
                    foreach (int rotation in new[] { 0, 1 })
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Move(rotation, translation);
                    }
                    break;
                case Tetromino.Z:
                    foreach (int rotation in new[] { 0, 1 })
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Move(rotation, translation);
                    }
                    break;
                case Tetromino.L:
                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Move(rotation, translation);
                    }
                    break;
                case Tetromino.J:
                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Move(rotation, translation);
                    }
                    break;
                case Tetromino.T:
                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        shape = Shape.Get(tetromino, rotation);
                        for (int translation = shape.TranslationMin; translation <= shape.TranslationMax; translation++)
                            yield return new Move(rotation, translation);
                    }
                    break;
                default:
                    break;
            };
        }

        public override string ToString()
        {
            return string.Format("Move {{ Rotation: {0}, Translation: {1}, Fall: {2} }}", Rotation, Translation, Fall);
        }
    }
}
