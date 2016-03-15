using GameBot.Core;
using GameBot.Emulation;
using SimpleInjector;
using System.Reflection;
using System.Linq;
using System;
using System.Drawing;
using System.Diagnostics;
using GameBot.Robot.Executors;
using GameBot.Robot.Renderers;
using GameBot.Robot.Quantizers;
using GameBot.Robot.Cameras;

namespace GameBot.Robot
{
    public class Launcher
    {
        static bool IsInteractive = false;
        static bool UseEmulator = true;

        static void Main(string[] args)
        {
            // Create dependency injection container
            var container = Bootstrapper.GetInitializedContainer(IsInteractive, UseEmulator);

            // Run the engine
            var engine = container.GetInstance<IEngine>();
            engine.Run();

            /*
            try
            {
                TestQuantizer();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }*/
        }

        static void TestQuantizer()
        {
            /*
            string path = "Images/tetris_1.jpg";
            var keypoints = new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } };
            var c = 5;
            var block = 13;
            */
            string path = "Images/tetris_2.jpg";
            var keypoints = new float[,] { { 321, 1677 }, { 2484, 1722 }, { 48, 3740 }, { 2826, 3758 } };
            var c = 5;
            var block = 13;

            var quantizer = new BinarizeQuantizer(true, keypoints, c, block);
            var image = Image.FromFile(path);

            var w = new Stopwatch();
            w.Start();
            var screenshot = quantizer.Quantize(image, TimeSpan.Zero);
            w.Stop();

            Debug.WriteLine("Elapsed miliseconds: " + w.ElapsedMilliseconds);
        }
    }
}
