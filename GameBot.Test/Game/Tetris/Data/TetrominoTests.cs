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
            var values = Enum.GetValues(typeof(Tetromino)).Cast<Tetromino>();
            Assert.AreEqual(7, values.Count());
        }
    }
}
