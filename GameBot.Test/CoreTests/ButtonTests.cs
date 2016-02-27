using GameBot.Core.Data;
using NUnit.Framework;
using System;
using System.Linq;

namespace GameBot.Test
{
    [TestFixture]
    public class ButtonTests
    {
        [Test]
        public void Size()
        {
            var values = Enum.GetValues(typeof(Button)).Cast<Button>();
            Assert.AreEqual(8, values.Count());
        }

        [Test]
        public void Values()
        {
            Assert.AreEqual(1, (int)Button.Up);
            Assert.AreEqual(2, (int)Button.Down);
            Assert.AreEqual(3, (int)Button.Left);
            Assert.AreEqual(4, (int)Button.Right);

            Assert.AreEqual(5, (int)Button.A);
            Assert.AreEqual(6, (int)Button.B);

            Assert.AreEqual(7, (int)Button.Start);
            Assert.AreEqual(8, (int)Button.Select);
        }
    }
}
