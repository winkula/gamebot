using GameBot.Core.Data;
using NUnit.Framework;
using System;

namespace GameBot.Test
{
    [TestFixture]
    public class CommandTests
    {
        [Test]
        public void Constructor()
        {
            var button = Button.Down;
            var timestamp = TimeSpan.FromSeconds(3);
            var duration = TimeSpan.FromMilliseconds(80);

            ICommand command = new Command(button, timestamp, duration);

            Assert.AreEqual(button, command.Button);
            Assert.AreEqual(duration, command.Duration);
            Assert.AreEqual(timestamp, command.Timestamp);
        }
    }
}
