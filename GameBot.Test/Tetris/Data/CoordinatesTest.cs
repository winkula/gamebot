using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System.Drawing;

namespace GameBot.Test.Tetris.Data
{
    [TestFixture]
    public class CoordinatesTests
    {
        [TestCase(0, 0, 2, 17)]
        [TestCase(-2, 17, 0, 0)]
        public void GameToTile(int x, int y, int xExpected, int yExpected)
        {
            var result = Coordinates.GameToTile(x, y);
            
            Assert.AreEqual(xExpected, result.X);
            Assert.AreEqual(yExpected, result.Y);

            result = Coordinates.GameToTile(new Point(x, y));

            Assert.AreEqual(xExpected, result.X);
            Assert.AreEqual(yExpected, result.Y);
        }

        //[TestCase(0, 0, 4, -1)]
        //[TestCase(-4, 1, 0, 0)]
        //public void GameToSearchWindow(int x, int y, int xExpected, int yExpected)
        //{
        //    var result = Coordinates.GameToSearchWindow(x, y);

        //    Assert.AreEqual(xExpected, result.X);
        //    Assert.AreEqual(yExpected, result.Y);

        //    result = Coordinates.GameToSearchWindow(new Point(x, y));

        //    Assert.AreEqual(xExpected, result.X);
        //    Assert.AreEqual(yExpected, result.Y);
        //}

        //[TestCase(5, 1, 0, 0)]
        //[TestCase(0, 0, -5, 1)]
        //public void TileToGame(int x, int y, int xExpected, int yExpected)
        //{
        //    var result = Coordinates.TileToGame(x, y);

        //    Assert.AreEqual(xExpected, result.X);
        //    Assert.AreEqual(yExpected, result.Y);

        //    result = Coordinates.TileToGame(new Point(x, y));

        //    Assert.AreEqual(xExpected, result.X);
        //    Assert.AreEqual(yExpected, result.Y);
        //}        
    }
}
