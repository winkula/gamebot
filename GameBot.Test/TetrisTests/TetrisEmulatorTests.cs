using GameBot.Core.Data;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;

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

            emulator.Execute(new Command(Button.Down));
            emulator.Execute(new Command(Button.Down));
            emulator.Execute(new Command(Button.Down));
            emulator.Execute(new Command(Button.Down));
            emulator.Execute(new Command(Button.Down));

            Debug.WriteLine(emulator.GameState);
        }
    }
}
