using GameBot.Core.Data;
using GameBot.Emulation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GameBot.Test.Emulation
{
    [TestFixture]
    public class EmulatorTests
    {
        private readonly bool _saveImages = false;

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
            var game = loader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(game);

            RunSimulation(emulator, list, _saveImages);
        }

        [Test]
        public void Rotate()
        {
            var list = new List<dynamic>();
            list.Add(new { Button = Button.Start, Duration = 2.5 });
            list.Add(new { Button = Button.Start, Duration = 2.7 });
            list.Add(new { Button = Button.Start, Duration = 0 });
            for (int i = 0; i < 4; i++)
            {
                list.Add(new { Button = Button.A, Duration = 0 });
            }

            var loader = new RomLoader();
            var game = loader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(game);

            RunSimulation(emulator, list, _saveImages);
        }

        private void RunSimulation(Emulator emulator, IEnumerable<dynamic> inputs, bool saveImages)
        {
            if (saveImages)
            {
                Clean();
                SaveImage(emulator.Display);
            }

            emulator.Execute(TimeSpan.FromSeconds(3));

            if (saveImages) { SaveImage(emulator.Display); }

            foreach (var input in inputs)
            {
                emulator.Hit(input.Button);
                emulator.Execute((TimeSpan) TimeSpan.FromSeconds(input.Duration));
                if (saveImages)
                {
                    SaveImage(emulator.Display);
                }
            }
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
                emulator.Hit(button);
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
