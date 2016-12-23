using System.Collections.Generic;
using System.Drawing;

namespace GameBot.Game.Tetris.Data
{
    /// <summary>
    /// Represents the shape and the body of a Tetrimino.
    /// Provides quick access to occupied blocks.
    /// </summary>
    public class Shape
    {
        public static readonly Shape O = Get(Tetrimino.O);
        public static readonly Shape I = Get(Tetrimino.I);
        public static readonly Shape S = Get(Tetrimino.S);
        public static readonly Shape Z = Get(Tetrimino.Z);
        public static readonly Shape L = Get(Tetrimino.L);
        public static readonly Shape J = Get(Tetrimino.J);
        public static readonly Shape T = Get(Tetrimino.T);

        private static Shape[,] _shapes;

        private static readonly int _localCoordinateMin = -1;
        private static readonly int _localCoordinateMax = 2;
        private static readonly int _localCoordinateSize = _localCoordinateMax - _localCoordinateMin + 1;

        // this masks are built from the screen tiles like this (the number represents the bits from lowest to heighest):
        //
        // 9 10 11 12
        // 5  6  7  8
        // 1  2  3  4
        //
        private static readonly ushort[] _bodies = {
            0x0066, 0x0066, 0x0066, 0x0066, // O
            0x00F0, 0x2222, 0x00F0, 0x2222, // I
            0x0063, 0x0132, 0x0063, 0x0132, // S
            0x0036, 0x0231, 0x0036, 0x0231, // Z
            0x0071, 0x0322, 0x0470, 0x0226, // L
            0x0074, 0x0223, 0x0170, 0x0622, // J
            0x0072, 0x0232, 0x0270, 0x0262  // T
        };

        // TODO: make this a extension method of tetrimino?
        public static Shape Get(Tetrimino tetrimino, int orientation = 0)
        {
            // TODO: remove this hack
            if (_shapes == null)
            {
                _shapes = new Shape[7, 4];

                _shapes[(int)Tetrimino.O, 0] = new Shape(Tetrimino.O, 0, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(1, -1), new Point(1, 0) }, new List<Point> { new Point(0, -1), new Point(1, -1) }, -4, 4, 2, 2);
                _shapes[(int)Tetrimino.O, 1] = new Shape(Tetrimino.O, 1, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(1, -1), new Point(1, 0) }, new List<Point> { new Point(0, -1), new Point(1, -1) }, -4, 4, 2, 2);
                _shapes[(int)Tetrimino.O, 2] = new Shape(Tetrimino.O, 2, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(1, -1), new Point(1, 0) }, new List<Point> { new Point(0, -1), new Point(1, -1) }, -4, 4, 2, 2);
                _shapes[(int)Tetrimino.O, 3] = new Shape(Tetrimino.O, 3, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(1, -1), new Point(1, 0) }, new List<Point> { new Point(0, -1), new Point(1, -1) }, -4, 4, 2, 2);

                _shapes[(int)Tetrimino.I, 0] = new Shape(Tetrimino.I, 0, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(2, 0) }, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(2, 0) }, -3, 3, 4, 1);
                _shapes[(int)Tetrimino.I, 1] = new Shape(Tetrimino.I, 1, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(0, 2) }, new List<Point> { new Point(0, -1) }, -4, 5, 1, 4);
                _shapes[(int)Tetrimino.I, 2] = new Shape(Tetrimino.I, 2, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(2, 0) }, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(2, 0) }, -3, 3, 4, 1);
                _shapes[(int)Tetrimino.I, 3] = new Shape(Tetrimino.I, 3, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(0, 2) }, new List<Point> { new Point(0, -1) }, -4, 5, 1, 4);

                _shapes[(int)Tetrimino.S, 0] = new Shape(Tetrimino.S, 0, new List<Point> { new Point(-1, -1), new Point(0, -1), new Point(0, 0), new Point(1, 0) }, new List<Point> { new Point(-1, -1), new Point(0, -1), new Point(1, 0) }, -3, 4, 3, 2);
                _shapes[(int)Tetrimino.S, 1] = new Shape(Tetrimino.S, 1, new List<Point> { new Point(-1, 0), new Point(-1, 1), new Point(0, -1), new Point(0, 0) }, new List<Point> { new Point(-1, 0), new Point(0, -1) }, -3, 5, 2, 3);
                _shapes[(int)Tetrimino.S, 2] = new Shape(Tetrimino.S, 2, new List<Point> { new Point(-1, -1), new Point(0, -1), new Point(0, 0), new Point(1, 0) }, new List<Point> { new Point(-1, -1), new Point(0, -1), new Point(1, 0) }, -3, 4, 3, 2);
                _shapes[(int)Tetrimino.S, 3] = new Shape(Tetrimino.S, 3, new List<Point> { new Point(-1, 0), new Point(-1, 1), new Point(0, -1), new Point(0, 0) }, new List<Point> { new Point(-1, 0), new Point(0, -1) }, -3, 5, 2, 3);

                _shapes[(int)Tetrimino.Z, 0] = new Shape(Tetrimino.Z, 0, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(0, 0), new Point(1, -1) }, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(1, -1) }, -3, 4, 3, 2);
                _shapes[(int)Tetrimino.Z, 1] = new Shape(Tetrimino.Z, 1, new List<Point> { new Point(-1, -1), new Point(-1, 0), new Point(0, 0), new Point(0, 1) }, new List<Point> { new Point(-1, -1), new Point(0, 0) }, -3, 5, 2, 3);
                _shapes[(int)Tetrimino.Z, 2] = new Shape(Tetrimino.Z, 2, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(0, 0), new Point(1, -1) }, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(1, -1) }, -3, 4, 3, 2);
                _shapes[(int)Tetrimino.Z, 3] = new Shape(Tetrimino.Z, 3, new List<Point> { new Point(-1, -1), new Point(-1, 0), new Point(0, 0), new Point(0, 1) }, new List<Point> { new Point(-1, -1), new Point(0, 0) }, -3, 5, 2, 3);

                _shapes[(int)Tetrimino.L, 0] = new Shape(Tetrimino.L, 0, new List<Point> { new Point(-1, -1), new Point(-1, 0), new Point(0, 0), new Point(1, 0) }, new List<Point> { new Point(-1, -1), new Point(0, 0), new Point(1, 0) }, -3, 4, 3, 2);
                _shapes[(int)Tetrimino.L, 1] = new Shape(Tetrimino.L, 1, new List<Point> { new Point(-1, 1), new Point(0, -1), new Point(0, 0), new Point(0, 1) }, new List<Point> { new Point(-1, 1), new Point(0, -1) }, -3, 5, 2, 3);
                _shapes[(int)Tetrimino.L, 2] = new Shape(Tetrimino.L, 2, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(1, 1) }, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0) }, -3, 4, 3, 2);
                _shapes[(int)Tetrimino.L, 3] = new Shape(Tetrimino.L, 3, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(1, -1) }, new List<Point> { new Point(0, -1), new Point(1, -1) }, -4, 4, 2, 3);

                _shapes[(int)Tetrimino.J, 0] = new Shape(Tetrimino.J, 0, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, -1), new Point(1, 0) }, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, -1) }, -3, 4, 3, 2);
                _shapes[(int)Tetrimino.J, 1] = new Shape(Tetrimino.J, 1, new List<Point> { new Point(-1, -1), new Point(0, -1), new Point(0, 0), new Point(0, 1) }, new List<Point> { new Point(-1, -1), new Point(0, -1) }, -3, 5, 2, 3);
                _shapes[(int)Tetrimino.J, 2] = new Shape(Tetrimino.J, 2, new List<Point> { new Point(-1, 0), new Point(-1, 1), new Point(0, 0), new Point(1, 0) }, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0) }, -3, 4, 3, 2);
                _shapes[(int)Tetrimino.J, 3] = new Shape(Tetrimino.J, 3, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(1, 1) }, new List<Point> { new Point(0, -1), new Point(1, 1) }, -4, 4, 2, 3);

                _shapes[(int)Tetrimino.T, 0] = new Shape(Tetrimino.T, 0, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(0, 0), new Point(1, 0) }, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(1, 0) }, -3, 4, 3, 2);
                _shapes[(int)Tetrimino.T, 1] = new Shape(Tetrimino.T, 1, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(0, 0), new Point(0, 1) }, new List<Point> { new Point(-1, 0), new Point(0, -1) }, -3, 5, 2, 3);
                _shapes[(int)Tetrimino.T, 2] = new Shape(Tetrimino.T, 2, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(0, 1), new Point(1, 0) }, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0) }, -3, 4, 3, 2);
                _shapes[(int)Tetrimino.T, 3] = new Shape(Tetrimino.T, 3, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(1, 0) }, new List<Point> { new Point(0, -1), new Point(1, 0) }, -4, 4, 2, 3);
            }
            return _shapes[(int)tetrimino, orientation];
        }

        // TODO: make constructor, better performance without loops, direct lookup!
        public static Shape FromMask(ushort mask)
        {
            // TODO: remove this ugly hack
            // special case, because shape is not completly visible
            if (mask == 0x0222) return Get(Tetrimino.I, 1);

            for (int i = 0; i < 7 * 4; i++)
            {
                if (mask == _bodies[i])
                {
                    var tetromino = (Tetrimino)(i / 4);
                    var orientation = i % 4;
                    return Get(tetromino, orientation);
                }
            }
            return null;
        }

        public Tetrimino Tetrimino { get; }
        public int Orientation { get; }

        // coordinates of occupied tiles of all possible Tetrimino and rotation possibilities
        public IEnumerable<Point> Body { get; }

        public IEnumerable<Point> Head { get; }

        public int TranslationMin { get; }
        public int TranslationMax { get; }

        public int Width { get; }
        public int Height { get; }

        private Shape(Tetrimino tetrimino, int orientation, IEnumerable<Point> body, IEnumerable<Point> head, int translationMin, int translationMax, int width, int height)
        {
            Tetrimino = tetrimino;
            Orientation = orientation;
            Body = body;
            Head = head;
            TranslationMin = translationMin;
            TranslationMax = translationMax;
            Width = width;
            Height = height;
        }

        // Local coordinates are used (relative to origin of the piece)
        public bool IsSquareOccupied(int x, int y)
        {
            if (x >= _localCoordinateMin && x <= _localCoordinateMax && y >= _localCoordinateMin && y <= _localCoordinateMax)
            {
                int index = ((int)Tetrimino) * 4 + Orientation;
                ushort matrix = _bodies[index];
                int bit = _localCoordinateSize * (y - _localCoordinateMin) + (x - _localCoordinateMin);
                int mask = 1 << bit;
                return (matrix & mask) != 0;
            }
            return false;
        }

        public Shape Rotate()
        {
            return _shapes[(int)Tetrimino, (Orientation + 1) % 4];
        }

        public Shape RotateCounterclockwise()
        {
            return _shapes[(int)Tetrimino, (Orientation + 3) % 4];
        }
    }
}
