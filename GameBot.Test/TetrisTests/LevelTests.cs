using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace GameBot.Test.TetrisTests
{
    [TestFixture]
    public class LevelTests
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
            var speed = Level.GetSpeed(level);

            Assert.AreEqual(expected, speed);
        }

        [TestCase(0, 10, 8.873)]
        [TestCase(1, 10, 8.203)]
        [TestCase(10, 10, 1.674)]
        [TestCase(20, 10, 0.502)]
        public void GetDuration(int level, int rows, double expected)
        {
            var duration = Level.GetDuration(level, rows);

            Assert.True(Math.Abs(duration.TotalSeconds - expected) < 0.01);
        }
    }
}
