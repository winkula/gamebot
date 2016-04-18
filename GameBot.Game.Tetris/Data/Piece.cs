using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace GameBot.Game.Tetris.Data
{
    public class Piece
    {
        private static readonly Random random = new Random();

        public Tetromino Tetromino { get; private set; }
        public Shape Shape { get; private set; }
        public int Orientation { get; private set; }
        public int X { get; private set; }
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

        public bool IsSquareOccupiedRegardTranslation(int x, int y)
        {
            return Shape.IsSquareOccupied(x - X, y - Y);
        }

        public override int GetHashCode()
        {
            // TODO: better hash code
            return (int)Tetromino ^ Orientation ^ X ^ Y;
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
