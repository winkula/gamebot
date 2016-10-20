using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System;

namespace GameBot.Test.Game.Tetris.Data
{
    [TestFixture]
    public class PieceDeltaTests
    {
        [Test]
        public void ConstructorFails()
        {
            var piece = new Piece();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var delta = new PieceDelta(piece, null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                var delta = new PieceDelta(null, piece);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                var delta = new PieceDelta(null, null);
            });
        }

        [Test]
        public void Constructor()
        {
            var piece1 = new Piece();
            var piece2 = new Piece();

            var delta = new PieceDelta(piece1, piece2);
        }
        
        [Test]
        public void Delta1()
        {
            var current = new Piece(Tetrimino.O, 1, 10, -4);
            var target = new Piece(Tetrimino.O, 2, 8, -7);

            var delta = new PieceDelta(current, target);

            Assert.AreEqual(1, delta.Orientation);
            Assert.AreEqual(-2, delta.X);
            Assert.AreEqual(-3, delta.Y);
        }

        [Test]
        public void Delta2()
        {
            var current = new Piece(Tetrimino.O, 3, 3, -4);
            var target = new Piece(Tetrimino.O, 1, 8, -1);

            var delta = new PieceDelta(current, target);

            Assert.AreEqual(-2, delta.Orientation);
            Assert.AreEqual(5, delta.X);
            Assert.AreEqual(3, delta.Y);
        }
    }
}
