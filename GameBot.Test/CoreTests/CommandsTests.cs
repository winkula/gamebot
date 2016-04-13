using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
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
            collection.Add(new HitCommand(Button.Left, new TimeSpan(), new TimeSpan()));
            collection.Add(new HitCommand(Button.Up, new TimeSpan(), new TimeSpan()));
            collection.Add(new HitCommand(Button.A, new TimeSpan(), new TimeSpan()));

            IEnumerable<ICommand> commands = new CommandCollection(collection);

            Assert.AreEqual(collection.Count, commands.ToList().Count);
        }

        [Test]
        public void ConstructorDefault()
        {
            IEnumerable<ICommand> commands = new CommandCollection();

            Assert.AreEqual(0, commands.ToList().Count);
        }
    }
}
