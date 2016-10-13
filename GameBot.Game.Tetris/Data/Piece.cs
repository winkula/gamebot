using System;

namespace GameBot.Game.Tetris.Data
{
    public class Piece
    {
        private static readonly Random random = new Random();

        public Tetromino Tetromino { get; private set; }
        public Shape Shape { get; private set; }

        /// <summary>
        /// Orientation is counted in clockwise direction from the start position of a Tetromino.
        /// </summary>
        public int Orientation { get; private set; }

        /// <summary>
        /// X axis is from left to right. The origin is the spawning point of the Tetromino.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Y axis is from bottom to top. The origin is the spawning point of the Tetromino.
        /// </summary>
        public int Y { get; private set; }

        public Piece(Tetromino tetromino, int orientation = 0, int x = 0, int y = 0)
        {
            if (orientation < 0 || orientation > 3) throw new ArgumentException("orientation must be between 0 and 3");

            Tetromino = tetromino;
            Shape = Shape.Get(tetromino, orientation);
            Orientation = orientation;
            X = x;
            Y = y;
        }

        public Piece(Piece piece) : this(piece.Tetromino, piece.Orientation, piece.X, piece.Y)
        {
        }

        public Piece() : this(Tetrominos.GetRandom(), 0, 0, 0)
        {
        }

        public static Piece FromMask(ushort mask)
        {
            var shape = Shape.FromMask(mask);
            if (shape != null)
            {
                return new Piece(shape.Tetromino, shape.Orientation);
            }
            return null;
        }

        // A button
        public Piece Rotate()
        {
            Orientation = (Orientation + 1) % 4;
            Shape = Shape.Rotate();
            return this;
        }

        // B button
        public Piece RotateCounterclockwise()
        {
            Orientation = (Orientation + 4 - 1) % 4;
            Shape = Shape.RotateCounterclockwise();
            return this;
        }

        public Piece Fall()
        {
            Y--;
            return this;
        }

        public Piece Fall(int distance)
        {
            if (distance < 0) throw new ArgumentException(nameof(distance));

            Y -= distance;
            return this;
        }

        public Piece Left()
        {
            X--;
            return this;
        }

        public Piece Right()
        {
            X++;
            return this;
        }

        public Piece Apply(Move move)
        {
            // TODO: lookup table
            switch (move)
            {
                case Move.None: break;
                case Move.Left: Left(); break;
                case Move.Right: Right(); break;
                case Move.Rotate: Rotate(); break;
                case Move.RotateCounterclockwise: RotateCounterclockwise(); break;
                case Move.Fall: Fall(); break;
                default:
                    throw new ArgumentException("only None, Left, Right, Rotate and RotateCounterclockwise are allowed.");
            }
            return this;
        }

        public bool IsSquareOccupiedRegardTranslation(int x, int y)
        {
            return Shape.IsSquareOccupied(x - X, y - Y);
        }

        public PieceDelta Delta(Piece target)
        {
            return new PieceDelta(this, target);
        }

        public override int GetHashCode()
        {
            return (int)Tetromino ^ (Orientation << 3) ^ (X << 5) ^ (Y << 10);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            Piece other = obj as Piece;
            if (other != null)
            {
                return Tetromino == other.Tetromino && Orientation == other.Orientation && X == other.X && Y == other.Y;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("Piece({0}, o:{1}, x:{2}, y:{3})", Tetromino, Orientation, X, Y);
        }
    }
}
