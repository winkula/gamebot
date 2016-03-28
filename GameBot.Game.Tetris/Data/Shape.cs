using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Game.Tetris.Data
{
    /// <summary>
    /// Represents the shape and the body of a Tetromino.
    /// Provides quick access to occupied blocks.
    /// </summary>
    public class Shape
    {
        public static readonly Shape O = Get(Tetromino.O);
        public static readonly Shape I = Get(Tetromino.I);
        public static readonly Shape S = Get(Tetromino.S);
        public static readonly Shape Z = Get(Tetromino.Z);
        public static readonly Shape L = Get(Tetromino.L);
        public static readonly Shape J = Get(Tetromino.J);
        public static readonly Shape T = Get(Tetromino.T);

        private static Shape[,] shapes;

        private static int LocalCoordinateMin = -1;
        private static int LocalCoordinateMax = 2;
        private static int LocalCoordinateSize = LocalCoordinateMax - LocalCoordinateMin + 1;
        private static ushort[] bodies = new ushort[] {
            0x0066, 0x0066, 0x0066, 0x0066, // O
            0x00F0, 0x2222, 0x00F0, 0x2222, // I
            0x0063, 0x0132, 0x0063, 0x0132, // S
            0x0036, 0x0231, 0x0036, 0x0231, // Z
            0x0071, 0x0322, 0x0470, 0x0226, // L
            0x0074, 0x0223, 0x0170, 0x0622, // J
            0x0072, 0x0232, 0x0270, 0x0262  // T
        };

        public static Shape Get(Tetromino tetromino, int orientation = 0)
        {
            // TODO: remove this hack
            if (shapes == null)
            {
                shapes = new Shape[7, 4];

                shapes[(int)Tetromino.O, 0] = new Shape(Tetromino.O, 0, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(1, -1), new Point(1, 0) }, new List<Point> { new Point(0, -1), new Point(1, -1) }, -4, 4);
                shapes[(int)Tetromino.O, 1] = new Shape(Tetromino.O, 1, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(1, -1), new Point(1, 0) }, new List<Point> { new Point(0, -1), new Point(1, -1) }, -4, 4);
                shapes[(int)Tetromino.O, 2] = new Shape(Tetromino.O, 2, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(1, -1), new Point(1, 0) }, new List<Point> { new Point(0, -1), new Point(1, -1) }, -4, 4);
                shapes[(int)Tetromino.O, 3] = new Shape(Tetromino.O, 3, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(1, -1), new Point(1, 0) }, new List<Point> { new Point(0, -1), new Point(1, -1) }, -4, 4);

                shapes[(int)Tetromino.I, 0] = new Shape(Tetromino.I, 0, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(2, 0) }, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(2, 0) }, -3, 3);
                shapes[(int)Tetromino.I, 1] = new Shape(Tetromino.I, 1, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(0, 2) }, new List<Point> { new Point(0, -1) }, -4, 5);
                shapes[(int)Tetromino.I, 2] = new Shape(Tetromino.I, 2, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(2, 0) }, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(2, 0) }, -3, 3);
                shapes[(int)Tetromino.I, 3] = new Shape(Tetromino.I, 3, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(0, 2) }, new List<Point> { new Point(0, -1) }, -4, 5);

                shapes[(int)Tetromino.S, 0] = new Shape(Tetromino.S, 0, new List<Point> { new Point(-1, -1), new Point(0, -1), new Point(0, 0), new Point(1, 0) }, new List<Point> { new Point(-1, -1), new Point(0, -1), new Point(1, 0) }, -3, 4);
                shapes[(int)Tetromino.S, 1] = new Shape(Tetromino.S, 1, new List<Point> { new Point(-1, 0), new Point(-1, 1), new Point(0, -1), new Point(0, 0) }, new List<Point> { new Point(-1, 0), new Point(0, -1) }, -3, 5);
                shapes[(int)Tetromino.S, 2] = new Shape(Tetromino.S, 2, new List<Point> { new Point(-1, -1), new Point(0, -1), new Point(0, 0), new Point(1, 0) }, new List<Point> { new Point(-1, -1), new Point(0, -1), new Point(1, 0) }, -3, 4);
                shapes[(int)Tetromino.S, 3] = new Shape(Tetromino.S, 3, new List<Point> { new Point(-1, 0), new Point(-1, 1), new Point(0, -1), new Point(0, 0) }, new List<Point> { new Point(-1, 0), new Point(0, -1) }, -3, 5);

                shapes[(int)Tetromino.Z, 0] = new Shape(Tetromino.Z, 0, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(0, 0), new Point(1, -1) }, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(1, -1) }, -3, 4);
                shapes[(int)Tetromino.Z, 1] = new Shape(Tetromino.Z, 1, new List<Point> { new Point(-1, -1), new Point(-1, 0), new Point(0, 0), new Point(0, 1) }, new List<Point> { new Point(-1, -1), new Point(0, 0) }, -3, 5);
                shapes[(int)Tetromino.Z, 2] = new Shape(Tetromino.Z, 2, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(0, 0), new Point(1, -1) }, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(1, -1) }, -3, 4);
                shapes[(int)Tetromino.Z, 3] = new Shape(Tetromino.Z, 3, new List<Point> { new Point(-1, -1), new Point(-1, 0), new Point(0, 0), new Point(0, 1) }, new List<Point> { new Point(-1, -1), new Point(0, 0) }, -3, 5);

                shapes[(int)Tetromino.L, 0] = new Shape(Tetromino.L, 0, new List<Point> { new Point(-1, -1), new Point(-1, 0), new Point(0, 0), new Point(1, 0) }, new List<Point> { new Point(-1, -1), new Point(0, 0), new Point(1, 0) }, -3, 4);
                shapes[(int)Tetromino.L, 1] = new Shape(Tetromino.L, 1, new List<Point> { new Point(-1, 1), new Point(0, -1), new Point(0, 0), new Point(0, 1) }, new List<Point> { new Point(-1, 1), new Point(0, -1) }, -3, 5);
                shapes[(int)Tetromino.L, 2] = new Shape(Tetromino.L, 2, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(1, 1) }, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0) }, -3, 4);
                shapes[(int)Tetromino.L, 3] = new Shape(Tetromino.L, 3, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(1, -1) }, new List<Point> { new Point(0, -1), new Point(1, -1) }, -4, 4);

                shapes[(int)Tetromino.J, 0] = new Shape(Tetromino.J, 0, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, -1), new Point(1, 0) }, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, -1) }, -3, 4);
                shapes[(int)Tetromino.J, 1] = new Shape(Tetromino.J, 1, new List<Point> { new Point(-1, -1), new Point(0, -1), new Point(0, 0), new Point(0, 1) }, new List<Point> { new Point(-1, -1), new Point(0, -1) }, -3, 5);
                shapes[(int)Tetromino.J, 2] = new Shape(Tetromino.J, 2, new List<Point> { new Point(-1, 0), new Point(-1, 1), new Point(0, 0), new Point(1, 0) }, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0) }, -3, 4);
                shapes[(int)Tetromino.J, 3] = new Shape(Tetromino.J, 3, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(1, 1) }, new List<Point> { new Point(0, -1), new Point(1, 1) }, -4, 4);

                shapes[(int)Tetromino.T, 0] = new Shape(Tetromino.T, 0, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(0, 0), new Point(1, 0) }, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(1, 0) }, -3, 4);
                shapes[(int)Tetromino.T, 1] = new Shape(Tetromino.T, 1, new List<Point> { new Point(-1, 0), new Point(0, -1), new Point(0, 0), new Point(0, 1) }, new List<Point> { new Point(-1, 0), new Point(0, -1) }, -3, 5);
                shapes[(int)Tetromino.T, 2] = new Shape(Tetromino.T, 2, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(0, 1), new Point(1, 0) }, new List<Point> { new Point(-1, 0), new Point(0, 0), new Point(1, 0) }, -3, 4);
                shapes[(int)Tetromino.T, 3] = new Shape(Tetromino.T, 3, new List<Point> { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(1, 0) }, new List<Point> { new Point(0, -1), new Point(1, 0) }, -4, 4);
            }
            return shapes[(int)tetromino, orientation];
        }

        // TODO: make constructor, better performance without loops, direct lookup!
        public static Shape FromMask(ushort mask)
        {
            for (int i = 0; i < 7 * 4; i++)
            {
                if (mask == bodies[i])
                {
                    var tetromino = (Tetromino)(i / 4);
                    var orientation = i % 4;
                    return Get(tetromino, orientation);
                }
            }
            return null;
        }

        public Tetromino Tetromino { get; }
        public int Orientation { get; }

        // coordinates of occupied tiles of all possible Tetromino and rotation possibilities
        public IEnumerable<Point> Body { get; }

        public IEnumerable<Point> Head { get; }

        public int TranslationMin { get; }
        public int TranslationMax { get; }

        public Shape(Tetromino tetromino, int orientation, IEnumerable<Point> body, IEnumerable<Point> head, int translationMin, int translationMax)
        {
            Tetromino = tetromino;
            Orientation = orientation;
            Body = body;
            Head = head;
            TranslationMin = translationMin;
            TranslationMax = translationMax;
        }

        // Local coordinates are used (relative to origin of the piece)
        public bool IsSquareOccupied(int x, int y)
        {
            if (x >= LocalCoordinateMin && x <= LocalCoordinateMax && y >= LocalCoordinateMin && y <= LocalCoordinateMax)
            {
                int index = ((int)Tetromino) * 4 + Orientation;
                ushort matrix = bodies[index];
                int bit = LocalCoordinateSize * (y - LocalCoordinateMin) + (x - LocalCoordinateMin);
                int mask = 1 << bit;
                return (matrix & mask) != 0;
            }
            return false;
        }

        public Shape Rotate()
        {
            return shapes[(int)Tetromino, (Orientation + 1) % 4];
        }

        public Shape RotateCounterclockwise()
        {
            return shapes[(int)Tetromino, (Orientation + 3) % 4];
        }
    }
}
