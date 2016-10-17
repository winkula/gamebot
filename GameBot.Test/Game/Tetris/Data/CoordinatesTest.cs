using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System.Drawing;

namespace GameBot.Test.Game.Tetris.Data
{
    [TestFixture]
    public class CoordinatesTests
    {
        [TestCase(0, 0, 2, 17)]
        [TestCase(-2, 17, 0, 0)]
        public void BoardToTile(int x, int y, int xExpected, int yExpected)
        {
            var result = Coordinates.BoardToTile(x, y);
            
            Assert.AreEqual(xExpected, result.X);
            Assert.AreEqual(yExpected, result.Y);

            result = Coordinates.BoardToTile(new Point(x, y));

            Assert.AreEqual(xExpected, result.X);
            Assert.AreEqual(yExpected, result.Y);
        }

        [TestCase(0, 0, 6, 1)]
        [TestCase(-6, 1, 0, 0)]
        public void PieceToTile(int x, int y, int xExpected, int yExpected)
        {
            var result = Coordinates.PieceToTile(x, y);

            Assert.AreEqual(xExpected, result.X);
            Assert.AreEqual(yExpected, result.Y);

            result = Coordinates.PieceToTile(new Point(x, y));

            Assert.AreEqual(xExpected, result.X);
            Assert.AreEqual(yExpected, result.Y);
        }

        [TestCase(0, 0, 5, -1)]
        [TestCase(-5, -1, 0, 0)]
        public void PieceToSearchWindow(int x, int y, int xExpected, int yExpected)
        {
            var result = Coordinates.PieceToTileSearchWindowOrigin(x, y);

            Assert.AreEqual(xExpected, result.X);
            Assert.AreEqual(yExpected, result.Y);

            result = Coordinates.PieceToTileSearchWindowOrigin(new Point(x, y));

            Assert.AreEqual(xExpected, result.X);
            Assert.AreEqual(yExpected, result.Y);
        }

        [Test]
        public void BoardToPiece()
        {
            int x = Coordinates.PieceOrigin.X;
            int y = Coordinates.PieceOrigin.Y;
            int xExpected = 0;
            int yExpected = 0;

            var result = Coordinates.BoardToPiece(x, y);

            Assert.AreEqual(xExpected, result.X);
            Assert.AreEqual(yExpected, result.Y);

            result = Coordinates.BoardToPiece(new Point(x, y));

            Assert.AreEqual(xExpected, result.X);
            Assert.AreEqual(yExpected, result.Y);
        }      
    }
}
