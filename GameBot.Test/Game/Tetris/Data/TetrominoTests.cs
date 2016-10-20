using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System;
using System.Linq;

namespace GameBot.Test.Game.Tetris.Data
{
    [TestFixture]
    public class TetrominoTests
    {
        [Test]
        public void Size()
        {
            var values = Enum.GetValues(typeof(Tetrimino)).Cast<Tetrimino>();
            Assert.AreEqual(7, values.Count());
        }
    }
}
