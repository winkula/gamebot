using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using GameBot.Game.Tetris;
using NLog;
using NUnit.Framework;

namespace GameBot.Test.Tetris
{
    [TestFixture]
    public class TetrisEmulatorTests
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void Constructor()
        {
            var emulator = new TetrisSimulator();
        }

        [Test]
        public void Press()
        {
            var emulator = new TetrisSimulator();
            
            logger.Info(emulator.GameState);

            emulator.Simulate(new HitCommand(Button.Down));
            emulator.Simulate(new HitCommand(Button.Down));
            emulator.Simulate(new HitCommand(Button.Down));
            emulator.Simulate(new HitCommand(Button.Down));
            emulator.Simulate(new HitCommand(Button.Down));

            logger.Info(emulator.GameState);
        }
    }
}
