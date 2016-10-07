﻿using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using Moq;
using NUnit.Framework;
using System;

namespace GameBot.Test.Core.Data.Commands
{
    [TestFixture]
    public class PressCommandTests
    {
        [Test]
        public void ConstructorOne()
        {
            var button = Button.Down;

            var command = new PressCommand(button);
            
            Assert.AreEqual(button, command.Button);
            Assert.AreEqual(TimeSpan.Zero, command.Timestamp);
        }
        
        [Test]
        public void ConstructorTwo()
        {
            var button = Button.Down;
            var timestamp = TimeSpan.FromSeconds(3);

            var command = new PressCommand(button, timestamp);

            Assert.AreEqual(button, command.Button);
            Assert.AreEqual(timestamp, command.Timestamp);
        }

        [Test]
        public void Execute()
        {
            int presses = 0;

            var actuatorRepo = new Mock<IActuator>();
            actuatorRepo.Setup(x => x.Hit(It.IsAny<Button>())).Callback(() => Assert.Fail());
            actuatorRepo.Setup(x => x.Press(It.IsAny<Button>())).Callback(() => presses++);
            actuatorRepo.Setup(x => x.Release(It.IsAny<Button>())).Callback(() => Assert.Fail());

            var button = Button.Down;
            var timestamp = TimeSpan.FromSeconds(3);
            var command = new PressCommand(button, timestamp);

            command.Execute(actuatorRepo.Object);

            Assert.AreEqual(1, presses);
        }
    }
}
