using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Test.Tetris.Data
{
    [TestFixture]
    public class ShapeTests
    {
        [Test]
        public void StaticAccess()
        {
            var o = Shape.O;

            Assert.AreEqual(Tetromino.O, o.Tetromino);
            Assert.AreEqual(0, o.Orientation);

            var i = Shape.I;

            Assert.AreEqual(Tetromino.I, i.Tetromino);
            Assert.AreEqual(0, i.Orientation);

            var s = Shape.S;

            Assert.AreEqual(Tetromino.S, s.Tetromino);
            Assert.AreEqual(0, s.Orientation);

            var z = Shape.Z;

            Assert.AreEqual(Tetromino.Z, z.Tetromino);
            Assert.AreEqual(0, z.Orientation);

            var l = Shape.L;

            Assert.AreEqual(Tetromino.L, l.Tetromino);
            Assert.AreEqual(0, l.Orientation);

            var j = Shape.J;

            Assert.AreEqual(Tetromino.J, j.Tetromino);
            Assert.AreEqual(0, j.Orientation);

            var t = Shape.T;

            Assert.AreEqual(Tetromino.T, t.Tetromino);
            Assert.AreEqual(0, t.Orientation);
        }

        [TestCase(Tetromino.O, new[] { 0, 1, 2, 3 }, new[] {
            0,0,0,0,
            0,0,0,0,
            0,1,1,0,
            0,2,2,0
        })]
        [TestCase(Tetromino.I, new[] { 0, 2 }, new[] {
            0,0,0,0,
            0,0,0,0,
            2,2,2,2,
            0,0,0,0
        })]
        [TestCase(Tetromino.I, new[] { 1, 3 }, new[] {
            0,1,0,0,
            0,1,0,0,
            0,1,0,0,
            0,2,0,0
        })]
        [TestCase(Tetromino.S, new[] { 0, 2 }, new[] {
            0,0,0,0,
            0,0,0,0,
            0,1,2,0,
            2,2,0,0
        })]
        [TestCase(Tetromino.S, new[] { 1, 3 }, new[] {
            0,0,0,0,
            1,0,0,0,
            2,1,0,0,
            0,2,0,0
        })]
        [TestCase(Tetromino.Z, new[] { 0, 2 }, new[] {
            0,0,0,0,
            0,0,0,0,
            2,1,0,0,
            0,2,2,0
        })]
        [TestCase(Tetromino.Z, new[] { 1, 3 }, new[] {
            0,0,0,0,
            0,1,0,0,
            1,2,0,0,
            2,0,0,0
        })]
        [TestCase(Tetromino.L, new[] { 0 }, new[] {
            0,0,0,0,
            0,0,0,0,
            1,2,2,0,
            2,0,0,0
        })]
        [TestCase(Tetromino.L, new[] { 1 }, new[] {
            0,0,0,0,
            2,1,0,0,
            0,1,0,0,
            0,2,0,0
        })]
        [TestCase(Tetromino.L, new[] { 2 }, new[] {
            0,0,0,0,
            0,0,1,0,
            2,2,2,0,
            0,0,0,0
        })]
        [TestCase(Tetromino.L, new[] { 3 }, new[] {
            0,0,0,0,
            0,1,0,0,
            0,1,0,0,
            0,2,2,0
        })]
        [TestCase(Tetromino.J, new[] { 0 }, new[] {
            0,0,0,0,
            0,0,0,0,
            2,2,1,0,
            0,0,2,0
        })]
        [TestCase(Tetromino.J, new[] { 1 }, new[] {
            0,0,0,0,
            0,1,0,0,
            0,1,0,0,
            2,2,0,0
        })]
        [TestCase(Tetromino.J, new[] { 2 }, new[] {
            0,0,0,0,
            1,0,0,0,
            2,2,2,0,
            0,0,0,0
        })]
        [TestCase(Tetromino.J, new[] { 3 }, new[] {
            0,0,0,0,
            0,1,2,0,
            0,1,0,0,
            0,2,0,0
        })]
        [TestCase(Tetromino.T, new[] { 0 }, new[] {
            0,0,0,0,
            0,0,0,0,
            2,1,2,0,
            0,2,0,0
        })]
        [TestCase(Tetromino.T, new[] { 1 }, new[] {
            0,0,0,0,
            0,1,0,0,
            2,1,0,0,
            0,2,0,0
        })]
        [TestCase(Tetromino.T, new[] { 2 }, new[] {
            0,0,0,0,
            0,1,0,0,
            2,2,2,0,
            0,0,0,0
        })]
        [TestCase(Tetromino.T, new[] { 3 }, new[] {
            0,0,0,0,
            0,1,0,0,
            0,1,2,0,
            0,2,0,0
        })]
        public void BodyAndHead(Tetromino tetromino, int[] orientations, int[] fields)
        {
            foreach (int orientation in orientations)
            {
                Piece piece = new Piece(tetromino, orientation);
                var body = piece.Shape.Body.ToList();
                var head = piece.Shape.Head.ToList();

                for (int x = -1; x < 3; x++)
                {
                    for (int y = -1; y < 3; y++)
                    {
                        bool expectedBody = fields[4 * (4 - 1 - (y + 1)) + (x + 1)] > 0;
                        bool expectedHead = fields[4 * (4 - 1 - (y + 1)) + (x + 1)] == 2;

                        bool occupied = piece.Shape.IsSquareOccupied(x, y);

                        Assert.AreEqual(expectedBody, occupied);

                        if (expectedBody)
                        {
                            Assert.Contains(new Point(x, y), body);
                        }
                        else
                        {
                            Assert.False(body.Contains(new Point(x, y)));
                        }

                        if (expectedHead)
                        {
                            Assert.Contains(new Point(x, y), head);
                        }
                        else
                        {
                            Assert.False(head.Contains(new Point(x, y)));
                        }
                    }
                }
            }
        }        
    }
}
