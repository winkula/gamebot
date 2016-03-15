using GameBot.Robot.Quantizers;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Drawing;

namespace GameBot.Test
{
    [TestFixture]
    class QuantizerTests
    {
        [Test]
        public void Quantize()
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

            var quantizer = new TestQuantizer(true, keypoints, c, block);
            var image = Image.FromFile(path);

            Assert.NotNull(image);

            var w = new Stopwatch();
            w.Start();
            var screenshot = quantizer.Quantize(image, TimeSpan.Zero);
            w.Stop();

            Debug.Write(w.ElapsedMilliseconds);

            Assert.NotNull(screenshot);
        }
    }
}
