using GameBot.Core.Data;
using GameBot.Emulation;
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
        public void Simulate()
        {
            var list = new List<Button>();
            list.Add(Button.Start);
            list.Add(Button.Start);
            list.Add(Button.Start);
            list.Add(Button.Left);
            list.Add(Button.Left);
            list.Add(Button.Left);
            list.Add(Button.Down);

            var loader = new RomLoader();
            var game = loader.Load(@"../../../Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(game);

            RunSimulation(emulator, list, false);
        }

        private void RunSimulation(Emulator emulator, IEnumerable<Button> buttons, bool saveImages)
        {
            if (saveImages)
            {
                Clean();
                SaveImage(emulator.Display);
            }

            emulator.Execute(TimeSpan.FromSeconds(3));

            if (saveImages) { SaveImage(emulator.Display); }

            foreach (var button in buttons)
            {
                emulator.KeyTyped(button);
                if (saveImages)
                {
                    SaveImage(emulator.Display);
                }
            }
        }

        private void Clean()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/GameBot_Emulator_Output";
            if (Directory.Exists(path))
            {
                foreach (var file in new DirectoryInfo(path).EnumerateFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        private void SaveImage(Image image)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/GameBot_Emulator_Output";
            string filename = path + "/display_" + DateTime.Now.Ticks + ".png";
            image.Save(filename, ImageFormat.Png);
        }
    }
}
