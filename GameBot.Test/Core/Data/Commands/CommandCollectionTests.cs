using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Test.Core.Data.Commands
{
    [TestFixture]
    public class CommandCollectionTests
    {
        [Test]
        public void ConstructorDefault()
        {
            IEnumerable<ICommand> collection = new CommandCollection();

            Assert.AreEqual(0, collection.Count());
        }

        [Test]
        public void Constructor()
        {
            var data = new List<ICommand>();
            data.Add(new HitCommand(Button.Left));
            data.Add(new HitCommand(Button.Up));
            data.Add(new HitCommand(Button.A));

            var collection = new CommandCollection(data);

            Assert.AreEqual(3, data.Count);
            Assert.AreEqual(3, collection.Count());
        }

        [Test]
        public void Add()
        {
            var collection = new CommandCollection();

            collection.Add(new PressCommand(Button.Select));

            Assert.AreEqual(1, collection.Count());
        }

        [Test]
        public void Hit()
        {
            var collection = new CommandCollection();
                        
            collection.Hit(Button.Right);
            collection.Hit(Button.Down, TimeSpan.Zero);
            collection.Hit(Button.Down, 0.5);

            Assert.AreEqual(3, collection.Count());
        }

        [Test]
        public void Press()
        {
            var collection = new CommandCollection();

            collection.Press(Button.Right);
            collection.Press(Button.Down, TimeSpan.Zero);
            collection.Press(Button.Down, 0.5);

            Assert.AreEqual(3, collection.Count());
        }

        [Test]
        public void Release()
        {
            var collection = new CommandCollection();

            collection.Release(Button.Right);
            collection.Release(Button.Down, TimeSpan.Zero);
            collection.Release(Button.Down, 0.5);

            Assert.AreEqual(3, collection.Count());
        }
    }
}
