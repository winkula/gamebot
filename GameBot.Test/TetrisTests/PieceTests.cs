using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace GameBot.Test.TetrisTests
{
    [TestFixture]
    public class PieceTests
    {
        [Test]
        public void Constructor()
        {
            var tetromino = Tetromino.S;
            var orientation = 1;
            var x = 1;
            var y = -4;

            var piece = new Piece(tetromino, orientation, x, y);

            Assert.AreEqual(tetromino, piece.Tetromino);
            Assert.AreEqual(orientation, piece.Orientation);
            Assert.AreEqual(x, piece.X);
            Assert.AreEqual(y, piece.Y);
        }

        [Test]
        public void ConstructorDefault()
        {
            var tetromino = Tetromino.S;

            var piece = new Piece(tetromino);

            Assert.AreEqual(tetromino, piece.Tetromino);
            Assert.AreEqual(0, piece.Orientation);
            Assert.AreEqual(0, piece.X);
            Assert.AreEqual(0, piece.Y);
        }

        [TestCase(Tetromino.O)]
        [TestCase(Tetromino.I)]
        [TestCase(Tetromino.S)]
        [TestCase(Tetromino.Z)]
        [TestCase(Tetromino.L)]
        [TestCase(Tetromino.J)]
        [TestCase(Tetromino.T)]
        public void Fall(Tetromino tetromino)
        {
            var piece = new Piece(tetromino);

            int yBefore = piece.Y;
            piece.Fall();
            int yAfter = piece.Y;

            Assert.AreEqual(yBefore, yAfter + 1);
        }

        [TestCase(Tetromino.O, 8)]
        [TestCase(Tetromino.I, 1)]
        [TestCase(Tetromino.S, 0)]
        [TestCase(Tetromino.Z, 1)]
        [TestCase(Tetromino.L, 2)]
        [TestCase(Tetromino.J, 6)]
        [TestCase(Tetromino.T, 3)]
        public void FallMultipleTimes(Tetromino tetromino, int times)
        {
            var piece1 = new Piece(tetromino);
            var piece2 = new Piece(tetromino);

            int yBefore1 = piece1.Y;
            int yBefore2 = piece2.Y;
            piece1.Fall(times);
            for (int i = 0; i < times; i++)
            {
                piece2.Fall();
            }
            int yAfter1 = piece1.Y;
            int yAfter2 = piece2.Y;


            Assert.AreEqual(yBefore1, yAfter1 + times);
            Assert.AreEqual(yBefore2, yAfter2 + times);
        }

        [TestCase(Tetromino.O, 8)]
        [TestCase(Tetromino.I, 1)]
        [TestCase(Tetromino.S, 0)]
        [TestCase(Tetromino.Z, 1)]
        [TestCase(Tetromino.L, 2)]
        [TestCase(Tetromino.J, 6)]
        [TestCase(Tetromino.T, 3)]
        public void Rotate(Tetromino tetromino, int times)
        {
            var piece = new Piece(tetromino);

            int orientationBefore = piece.Orientation;
            for (int i = 0; i < times; i++)
            {
                piece.Rotate();
            }
            int orientationAfter = piece.Orientation;

            Assert.AreEqual(orientationBefore, (orientationAfter + 4 - times) % 4);
        }

        [Test]
        public void Equals()
        {
            var piece1 = new Piece(Tetromino.J);
            var piece2 = new Piece(Tetromino.J);

            Assert.AreEqual(piece1, piece2);

            piece1 = new Piece(Tetromino.O, 1);
            piece2 = new Piece(Tetromino.O, 1);

            Assert.AreEqual(piece1, piece2);

            piece1 = new Piece(Tetromino.Z, 3, 7, 2);
            piece2 = new Piece(Tetromino.Z, 3, 7, 2);

            Assert.AreEqual(piece1, piece2);
        }

        [Test]
        public void NotEquals()
        {
            var piece1 = new Piece(Tetromino.J);
            var piece2 = new Piece(Tetromino.O);

            Assert.AreNotEqual(piece1, piece2);

            piece1 = new Piece(Tetromino.O, 1);
            piece2 = new Piece(Tetromino.O, 2);

            Assert.AreNotEqual(piece1, piece2);

            piece1 = new Piece(Tetromino.Z, 3, 6, 2);
            piece2 = new Piece(Tetromino.Z, 3, 7, 1);

            Assert.AreNotEqual(piece1, piece2);
        }
    }
}
