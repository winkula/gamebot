using System;

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
            0x0322, 0x0470, 0x0226, 0x0071, // L
            0x0223, 0x0170, 0x0622, 0x0074, // J
            0x0072, 0x0232, 0x0270, 0x0262  // T
        };

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
    }
}
