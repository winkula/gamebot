using GameBot.Game.Tetris.Data;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Data
{
    [TestFixture]
    public class PieceTests
    {
        [Test]
        public void Constructor()
        {
            var tetromino = Tetrimino.S;
            var orientation = 1;
            var x = 1;
            var y = -4;

            var piece = new Piece(tetromino, orientation, x, y);

            Assert.AreEqual(tetromino, piece.Tetrimino);
            Assert.AreEqual(orientation, piece.Orientation);
            Assert.AreEqual(x, piece.X);
            Assert.AreEqual(y, piece.Y);
        }

        [Test]
        public void ConstructorDefault()
        {
            var tetromino = Tetrimino.S;

            var piece = new Piece(tetromino);

            Assert.AreEqual(tetromino, piece.Tetrimino);
            Assert.AreEqual(0, piece.Orientation);
            Assert.AreEqual(0, piece.X);
            Assert.AreEqual(0, piece.Y);
        }

        [TestCase(Tetrimino.O)]
        [TestCase(Tetrimino.I)]
        [TestCase(Tetrimino.S)]
        [TestCase(Tetrimino.Z)]
        [TestCase(Tetrimino.L)]
        [TestCase(Tetrimino.J)]
        [TestCase(Tetrimino.T)]
        public void Fall(Tetrimino tetrimino)
        {
            var piece = new Piece(tetrimino);

            int yBefore = piece.Y;
            piece.Fall();
            int yAfter = piece.Y;

            Assert.AreEqual(yBefore, yAfter + 1);
        }

        [TestCase(Tetrimino.O, 8)]
        [TestCase(Tetrimino.I, 1)]
        [TestCase(Tetrimino.S, 0)]
        [TestCase(Tetrimino.Z, 1)]
        [TestCase(Tetrimino.L, 2)]
        [TestCase(Tetrimino.J, 6)]
        [TestCase(Tetrimino.T, 3)]
        public void FallMultipleTimes(Tetrimino tetrimino, int times)
        {
            var piece1 = new Piece(tetrimino);
            var piece2 = new Piece(tetrimino);

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

        [TestCase(Tetrimino.O, 8)]
        [TestCase(Tetrimino.I, 1)]
        [TestCase(Tetrimino.S, 0)]
        [TestCase(Tetrimino.Z, 1)]
        [TestCase(Tetrimino.L, 2)]
        [TestCase(Tetrimino.J, 6)]
        [TestCase(Tetrimino.T, 3)]
        public void Rotate(Tetrimino tetrimino, int times)
        {
            var piece = new Piece(tetrimino);

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
            var piece1 = new Piece(Tetrimino.J);
            var piece2 = new Piece(Tetrimino.J);

            Assert.AreEqual(piece1, piece2);

            piece1 = new Piece(Tetrimino.O, 1);
            piece2 = new Piece(Tetrimino.O, 1);

            Assert.AreEqual(piece1, piece2);

            piece1 = new Piece(Tetrimino.Z, 3, 7, -2);
            piece2 = new Piece(Tetrimino.Z, 3, 7, -2);

            Assert.AreEqual(piece1, piece2);
        }

        [Test]
        public void NotEquals()
        {
            var piece1 = new Piece(Tetrimino.J);
            var piece2 = new Piece(Tetrimino.O);

            Assert.AreNotEqual(piece1, piece2);

            piece1 = new Piece(Tetrimino.O, 1);
            piece2 = new Piece(Tetrimino.O, 2);

            Assert.AreNotEqual(piece1, piece2);

            piece1 = new Piece(Tetrimino.Z, 3, 6, -2);
            piece2 = new Piece(Tetrimino.Z, 3, 7, -1);

            Assert.AreNotEqual(piece1, piece2);
        }
    }
}
