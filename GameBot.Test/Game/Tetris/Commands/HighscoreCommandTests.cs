using System;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Commands;
using GameBot.Test.Engine.Physical.Actuators;
using Moq;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Commands
{
    [TestFixture]
    public class HighscoreCommandTests
    {
        private Mock<IExecutor> _executorMock;
        private HighscoreSimulator _highscoreSimulator;

        [SetUp]
        public void Init()
        {
            _executorMock = new Mock<IExecutor>();
            _executorMock.Setup(x => x.Hit(It.IsAny<Button>())).Callback<Button>(b => _highscoreSimulator.Handle(b));
            _executorMock.Setup(x => x.HitWait(It.IsAny<Button>(), It.IsAny<TimeSpan>())).Callback<Button, TimeSpan>((b, t) => _highscoreSimulator.Handle(b));

            _highscoreSimulator = new HighscoreSimulator();
        }

        [TestCase("G-BOT")]
        [TestCase("ABCDEF")]
        [TestCase("+-_D")]
        public void Write(string name)
        {
            var command = new HighscoreCommand(_executorMock.Object, name);
            command.Execute();

            Assert.AreEqual(name.PadRight(6, 'A'), _highscoreSimulator.Result);
        }
        
        [TestCase("bhu")]
        [TestCase("KJHLKNDHLKJ")]
        [TestCase("*@")]
        public void InvalidInput(string name)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var command = new HighscoreCommand(_executorMock.Object, name);
            });
        }

        [TestCase(null)]
        public void NullInput(string name)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new HighscoreCommand(_executorMock.Object, name);
            });
        }
    }
}
