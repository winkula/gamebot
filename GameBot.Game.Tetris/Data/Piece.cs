using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace GameBot.Game.Tetris.Data
{
    public class Piece
    {
        public static int CoordinateMin = -1;
        public static int CoordinateMax = 2;
        public static int CoordinateSize = CoordinateMax - CoordinateMin + 1;
        private static ushort[] pieces = new ushort[] {
            0x0066, 0x0066, 0x0066, 0x0066, // O
            0x00F0, 0x2222, 0x00F0, 0x2222, // I
            0x0063, 0x0132, 0x0063, 0x0132, // S
            0x0036, 0x0231, 0x0036, 0x0231, // Z
            0x0071, 0x0322, 0x0470, 0x0226, // L
            0x0074, 0x0223, 0x0170, 0x0622, // J
            0x0072, 0x0232, 0x0270, 0x0262  // T
        };

        // coordinates of occupied tiles of all possible Tetromino and rotation possibilities
        private readonly static IEnumerable<Point>[] pieceDataLookup;

        static Piece()
        {
            pieceDataLookup = new IEnumerable<Point>[4 * 7];
            // O
            pieceDataLookup[0] = new List<Point> { new Point(1, 2), new Point(2, 2), new Point(1, 3), new Point(2, 3) };
            pieceDataLookup[1] = new List<Point> { new Point(1, 2), new Point(2, 2), new Point(1, 3), new Point(2, 3) };
            pieceDataLookup[2] = new List<Point> { new Point(1, 2), new Point(2, 2), new Point(1, 3), new Point(2, 3) };
            pieceDataLookup[3] = new List<Point> { new Point(1, 2), new Point(2, 2), new Point(1, 3), new Point(2, 3) };
            // I
            pieceDataLookup[4] = new List<Point> { new Point(0, 2), new Point(1, 2), new Point(2, 2), new Point(3, 2) };
            pieceDataLookup[5] = new List<Point> { new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(1, 3) };
            pieceDataLookup[6] = new List<Point> { new Point(0, 2), new Point(1, 2), new Point(2, 2), new Point(3, 2) };
            pieceDataLookup[7] = new List<Point> { new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(1, 3) };
            // S
            pieceDataLookup[8] = new List<Point> { new Point(1, 2), new Point(2, 2), new Point(0, 3), new Point(1, 3) };
            pieceDataLookup[9] = new List<Point> { new Point(0, 1), new Point(0, 2), new Point(1, 2), new Point(1, 3) };
            pieceDataLookup[10] = new List<Point> { new Point(1, 2), new Point(2, 2), new Point(0, 3), new Point(1, 3) };
            pieceDataLookup[11] = new List<Point> { new Point(0, 1), new Point(0, 2), new Point(1, 2), new Point(1, 3) };
            // Z
            pieceDataLookup[12] = new List<Point> { new Point(0, 2), new Point(1, 2), new Point(1, 3), new Point(2, 3) };
            pieceDataLookup[13] = new List<Point> { new Point(1, 1), new Point(0, 2), new Point(1, 2), new Point(0, 3) };
            pieceDataLookup[14] = new List<Point> { new Point(0, 2), new Point(1, 2), new Point(1, 3), new Point(2, 3) };
            pieceDataLookup[15] = new List<Point> { new Point(1, 1), new Point(0, 2), new Point(1, 2), new Point(0, 3) };
            // L
            pieceDataLookup[16] = new List<Point> { new Point(0, 2), new Point(1, 2), new Point(2, 2), new Point(0, 3) };
            pieceDataLookup[17] = new List<Point> { new Point(0, 1), new Point(1, 1), new Point(1, 2), new Point(1, 3) };
            pieceDataLookup[18] = new List<Point> { new Point(2, 1), new Point(0, 2), new Point(1, 2), new Point(2, 2) };
            pieceDataLookup[19] = new List<Point> { new Point(1, 1), new Point(1, 2), new Point(1, 3), new Point(2, 3) };
            // J
            pieceDataLookup[20] = new List<Point> { new Point(0, 2), new Point(1, 2), new Point(2, 2), new Point(2, 3) };
            pieceDataLookup[21] = new List<Point> { new Point(1, 1), new Point(1, 2), new Point(0, 3), new Point(1, 3) };
            pieceDataLookup[22] = new List<Point> { new Point(0, 1), new Point(0, 2), new Point(1, 2), new Point(2, 2) };
            pieceDataLookup[23] = new List<Point> { new Point(1, 1), new Point(2, 1), new Point(1, 2), new Point(1, 3) };
            // T
            pieceDataLookup[24] = new List<Point> { new Point(0, 2), new Point(1, 2), new Point(2, 2), new Point(1, 3) };
            pieceDataLookup[25] = new List<Point> { new Point(1, 1), new Point(0, 2), new Point(1, 2), new Point(1, 3) };
            pieceDataLookup[26] = new List<Point> { new Point(1, 1), new Point(0, 2), new Point(1, 2), new Point(2, 2) };
            pieceDataLookup[27] = new List<Point> { new Point(1, 1), new Point(1, 2), new Point(2, 2), new Point(1, 3) };
        }

        public Tetromino Tetromino { get; private set; }
        public int Orientation { get; private set; }
        public int X { get; private set; }
        public int Y { get; set; }

        public Piece(Tetromino tetromino, int orientation = 0, int x = 0, int y = 0)
        {
            if (orientation < 0 || orientation > 3) throw new ArgumentException("orientation must be between 0 and 3");

            Tetromino = tetromino;
            Orientation = orientation;
            X = x;
            Y = y;
        }

        public Piece(Piece piece) : this(piece.Tetromino, piece.Orientation, piece.X, piece.Y)
        {
        }

        // A button
        public Piece Rotate()
        {
            Orientation = (Orientation + 1) % 4;
            return this;
        }

        // B button
        public Piece RotateCounterclockwise()
        {
            Orientation = (Orientation + 4 - 1) % 4;
            return this;
        }

        public Piece Fall()
        {
            Y--;
            return this;
        }

        public Piece Fall(int times)
        {
            if (times < 0) throw new ArgumentException(nameof(times));
            Y -= times;
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

        // Local coordinates are used (relative to origin of the piece)
        public bool IsSquareOccupied(int x, int y)
        {
            if (x >= CoordinateMin && x <= CoordinateMax && y >= CoordinateMin && y <= CoordinateMax)
            {
                int index = ((int)Tetromino) * 4 + Orientation;
                ushort matrix = pieces[index];
                int bit = CoordinateSize * (y - CoordinateMin) + (x - CoordinateMin);
                int mask = 1 << bit;
                return (matrix & mask) != 0;
            }
            return false;
        }

        public IEnumerable<Point> GetOccupiedSquares()
        {
            return pieceDataLookup[((int)Tetromino) * 4 + Orientation];
        }

        // TODO: make constructor, better performance without loops, direct lookup!
        public static Piece FromMask(ushort mask)
        {
            for (int i = 0; i < 7 * 4; i++)
            {
                var piece = pieces[i];
                if (mask == piece)
                {
                    var tetromino = (Tetromino)(i / 4);
                    var orientation = i % 4;
                    return new Piece(tetromino, orientation);
                }
            }
            return null;
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
