using GameBot.Game.Tetris;
using NUnit.Framework;
using System;

namespace GameBot.Test.Game.Tetris
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
        public void GetSpeed(int level, int expected)
        {
            var speed = TetrisLevel.GetFramesPerRow(level);

            Assert.AreEqual(expected, speed);
        }

        [TestCase(-1)]
        [TestCase(21)]
        [TestCase(35)]
        public void GetSpeedFails(int level)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var speed = TetrisLevel.GetFramesPerRow(level);
            });
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
        [TestCase(5, 0, 5)]
        [TestCase(5, 59, 5)]
        [TestCase(5, 60, 6)]
        [TestCase(5, 74, 7)]
        [TestCase(5, 666, 20)]
        [TestCase(9, 0, 9)]
        [TestCase(9, 22, 9)]
        [TestCase(9, 22, 9)]
        [TestCase(9, 160, 16)]
        [TestCase(9, 378, 20)]
        public void GetLevelAType(int startLevel, int clearedLines, int expectedLevel)
        {
            int level = TetrisLevel.GetLevel(startLevel, clearedLines);

            Assert.AreEqual(expectedLevel, level);
        }

        [TestCase(0, 0.8, 1)]
        [TestCase(0, 1.0, 2)]
        [TestCase(0, 20.0, 23)]
        [TestCase(20, 1.0, 20)]
        public void GetFallDistance(int level, double seconds, int expectedDistance)
        {
            var duration = TimeSpan.FromSeconds(seconds);
            int distance = TetrisLevel.GetFallDistance(level, duration);

            Assert.AreEqual(expectedDistance, distance);
        }
    }
}
