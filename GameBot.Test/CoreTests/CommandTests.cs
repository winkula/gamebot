using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
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
            var press = TimeSpan.FromSeconds(3);
            var duration = TimeSpan.FromMilliseconds(80);

            ICommand command = new HitCommand(button, press, duration);

            Assert.AreEqual(button, command.Button);
            Assert.AreEqual(press, command.Press);
            Assert.AreEqual(press + duration, command.Release);
        }
    }
}
