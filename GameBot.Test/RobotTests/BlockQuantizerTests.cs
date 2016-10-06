using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using GameBot.Robot.Quantizers;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace GameBot.Test.RobotTests
{
    [TestFixture]
    public class BlockQuantizerTests
    {
        private const bool adjust = false;
        
        [Test]
        public void BlockQuantizer()
        {
            string path = "Images/tetris_1.jpg";
            var keypoints = new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } };

            TestQuantizer(path, new BlockQuantizer(adjust, keypoints, 5, 13));
        }

        private void TestQuantizer(string path, IQuantizer quantizer)
        {
            var image = new Mat(path, LoadImageType.Grayscale);

            Assert.NotNull(image);

            var w = new Stopwatch();
            w.Start();
            var quantized = quantizer.Quantize(image);
            w.Stop();

            Debug.Write(w.ElapsedMilliseconds);

            Assert.NotNull(quantized);
        }
    }
}
