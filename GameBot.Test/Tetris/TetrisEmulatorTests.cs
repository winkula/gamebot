using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using GameBot.Game.Tetris;
using NUnit.Framework;
using System.Diagnostics;

namespace GameBot.Test.Tetris
{
    [TestFixture]
    public class TetrisEmulatorTests
    {
        [Test]
        public void Constructor()
        {
            var emulator = new TetrisSimulator();
        }

        [Test]
        public void Press()
        {
            var emulator = new TetrisSimulator();
            
            Debug.WriteLine(emulator.GameState);

            emulator.Simulate(new HitCommand(Button.Down));
            emulator.Simulate(new HitCommand(Button.Down));
            emulator.Simulate(new HitCommand(Button.Down));
            emulator.Simulate(new HitCommand(Button.Down));
            emulator.Simulate(new HitCommand(Button.Down));

            Debug.WriteLine(emulator.GameState);
        }
    }
}
