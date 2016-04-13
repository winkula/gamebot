using GameBot.Game.Tetris;
using NUnit.Framework;
using System;

namespace GameBot.Test.TetrisTests
{
    [TestFixture]
    public class TetrisLevelTests
    {
        [TestCase(0, 53)]
        [TestCase(1, 49)]
        [TestCase(2, 45)]
        [TestCase(18, 4)]
        [TestCase(19, 4)]
        [TestCase(20, 3)]
        [TestCase(30, 3)]
        public void GetSpeed(int level, int expected)
        {
            var speed = TetrisLevel.GetSpeed(level);

            Assert.AreEqual(expected, speed);
        }

        [TestCase(0, 10, 8.873)]
        [TestCase(1, 10, 8.203)]
        [TestCase(10, 10, 1.674)]
        [TestCase(20, 10, 0.502)]
        public void GetDuration(int level, int rows, double expected)
        {
            var duration = TetrisLevel.GetDuration(level, rows);

            Assert.True(Math.Abs(duration.TotalSeconds - expected) < 0.01);
        }

        [TestCase(0, 0, 0)]
        [TestCase(0, 5, 0)]
        [TestCase(0, 9, 0)]
        [TestCase(0, 10, 1)]
        [TestCase(0, 15, 1)]
        [TestCase(0, 19, 1)]
        [TestCase(0, 20, 2)]
        [TestCase(3, 0, 3)]
        [TestCase(3, 5, 3)]
        [TestCase(3, 9, 3)]
        [TestCase(3, 10, 4)]
        [TestCase(3, 15, 4)]
        [TestCase(3, 19, 4)]
        [TestCase(3, 20, 5)]
        public void GetLevelAType(int startLevel, int clearedLines, int expectedLevel)
        {
            int level = TetrisLevel.GetLevel(startLevel, clearedLines);

            Assert.AreEqual(expectedLevel, level);
        }
    }
}
