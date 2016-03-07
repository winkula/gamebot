using GameBot.Core.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Test
{
    [TestFixture]
    public class CommandsTests
    {
        [Test]
        public void Constructor()
        {
            var collection = new List<ICommand>();
            collection.Add(new Command(Button.Left, new TimeSpan(), new TimeSpan()));
            collection.Add(new Command(Button.Up, new TimeSpan(), new TimeSpan()));
            collection.Add(new Command(Button.A, new TimeSpan(), new TimeSpan()));

            ICommands commands = new Commands(collection);

            Assert.AreEqual(collection.Count, commands.ToList().Count);
        }

        [Test]
        public void ConstructorDefault()
        {
            ICommands commands = new Commands();

            Assert.AreEqual(0, commands.ToList().Count);
        }
    }
}
