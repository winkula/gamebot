using GameBot.Core.Data;
using GameBot.Game.Tetris;
using NUnit.Framework;
using System.Diagnostics;

namespace GameBot.Test.TetrisTests
{
    [TestFixture]
    public class TetrisEmulatorTests
    {
        [Test]
        public void Constructor()
        {
            var emulator = new TetrisEmulator();
        }

        [Test]
        public void Press()
        {
            var emulator = new TetrisEmulator();
            
            Debug.WriteLine(emulator.GameState);

            emulator.Execute(new HitCommand(Button.Down));
            emulator.Execute(new HitCommand(Button.Down));
            emulator.Execute(new HitCommand(Button.Down));
            emulator.Execute(new HitCommand(Button.Down));
            emulator.Execute(new HitCommand(Button.Down));

            Debug.WriteLine(emulator.GameState);
        }
    }
}
