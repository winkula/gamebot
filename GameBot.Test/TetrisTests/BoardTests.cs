using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;

namespace GameBot.Test
{
    [TestFixture]
    public class BoardTests
    {
        [Test]
        public void Constructor()
        {
            int width = 3;
            int height = 7;

            var board = new Board(width, height);

            Assert.AreEqual(width, board.Width);
            Assert.AreEqual(height, board.Height);
        }

        [Test]
        public void ConstructorDefault()
        {
            var board = new Board();

            Assert.AreEqual(10, board.Width);
            Assert.AreEqual(19, board.Height);

            Debug.WriteLine(board.ToString());
        }

        [Test]
        public void Place()
        {
            var board = new Board();
            var piece = new Piece(Tetromino.I);
            piece.Rotate();

            board.Place(piece);
                    
            Assert.AreEqual(10, board.Width);
            Assert.AreEqual(19, board.Height);

            Debug.WriteLine(board.ToString());
        }
    }
}
