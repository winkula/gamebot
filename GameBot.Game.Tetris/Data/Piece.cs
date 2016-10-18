using System;

namespace GameBot.Game.Tetris.Data
{
    public class Piece
    {
        public Tetromino Tetromino { get; }
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

        public int FallHeight => -Y;

        /// <summary>
        /// The piece is untouched, when it's not moved or rotated. But the piece could have fallen.
        /// </summary>
        public bool IsUntouched => Orientation == 0 && X == 0;

        public bool IsFallen => Y < 0;

        public bool IsOrigin => IsUntouched && !IsFallen;

        public Piece(Tetromino tetromino, int orientation = 0, int x = 0, int y = 0)
        {
            if (orientation < 0 || orientation > 3)
                throw new ArgumentException("orientation must be between 0 and 3");
            if (y > 0)
                throw new ArgumentException("y must be 0 or negative");

            Tetromino = tetromino;
            Shape = Shape.Get(tetromino, orientation);
            Orientation = orientation;
            X = x;
            Y = y;
        }

        public Piece(Piece piece) : this(piece.Tetromino, piece.Orientation, piece.X, piece.Y)
        {
        }

        public Piece() : this(Tetrominos.GetRandom())
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
            // we could use a lookup table to gain a little bit of performance
            switch (move)
            {
                case Move.Left: Left(); break;
                case Move.Right: Right(); break;
                case Move.Rotate: Rotate(); break;
                case Move.RotateCounterclockwise: RotateCounterclockwise(); break;
                case Move.Fall: Fall(); break;
                case Move.Drop: throw new ArgumentException("Drop is not possible on the piece itself");
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
            return $"Piece({Tetromino}, o:{Orientation}, x:{X}, y:{Y})";
        }
    }
}
