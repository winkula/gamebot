using GameBot.Core;
using GameBot.Emulator;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GameBot.Test
{
    [TestFixture]
    class EmulatorTests
    {
        [Test]
        public void Test()
        {
            var list = new List<Buttons>();
            list.Add(Buttons.Start);
            list.Add(Buttons.Start);
            list.Add(Buttons.Start);
            list.Add(Buttons.Left);
            list.Add(Buttons.Left);
            list.Add(Buttons.Left);
            list.Add(Buttons.Down);

            var loader = new RomLoader();
            var game = loader.Load(@"..\..\..\Roms\tetris.gb");

            var emulator = new GameBoyEmulator();
            emulator.Load(game);

            RunSimulation(emulator, list);
        }

        private void RunSimulation(GameBoyEmulator emulator, IEnumerable<Buttons> buttons)
        {
            Clean();

            SaveImage(emulator.Display);

            emulator.Execute(TimeSpan.FromSeconds(3));
            SaveImage(emulator.Display);

            foreach (var button in buttons)
            {
                emulator.KeyTyped(button);
                SaveImage(emulator.Display);
            }
        }

        private void Clean()
        {
            foreach (var file in new DirectoryInfo(@"C:\Users\Winkler\Desktop\out").GetFiles())
            {
                file.Delete();
            }
        }

        private void SaveImage(Image image)
        {
            string filename = string.Format(@"C:\Users\Winkler\Desktop\out\{0}display.png", DateTime.Now.Ticks);
            image.Save(filename, ImageFormat.Png);
        }
    }
}
