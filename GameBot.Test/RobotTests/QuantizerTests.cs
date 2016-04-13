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
    public class QuantizerTests
    {
        private const bool adjust = false;

        [Test]
        public void UnwarpAndAdaptiveThreshold1()
        {
            string path = "Images/tetris_1.jpg";
            var keypoints = new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } };

            TestQuantizer(path, new AdaptiveThresholdQuantizer(adjust, keypoints, 5, 13));
        }

        [Test]
        public void UnwarpAndThreshold1()
        {
            string path = "Images/tetris_1.jpg";
            var keypoints = new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } };

            TestQuantizer(path, new ThresholdQuantizer(adjust, keypoints, 135));
        }

        [Test]
        public void UnwarpAndAdaptiveThreshold2()
        {
            string path = "Images/tetris_2.jpg";
            var keypoints = new float[,] { { 321, 1677 }, { 2484, 1722 }, { 48, 3740 }, { 2826, 3758 } };

            TestQuantizer(path, new AdaptiveThresholdQuantizer(adjust, keypoints, 5, 13));
        }

        [Test]
        public void UnwarpAndThreshold2()
        {
            string path = "Images/tetris_2.jpg";
            var keypoints = new float[,] { { 321, 1677 }, { 2484, 1722 }, { 48, 3740 }, { 2826, 3758 } };

            TestQuantizer(path, new ThresholdQuantizer(adjust, keypoints, 50));
        }

        private void TestQuantizer(string path, IQuantizer quantizer)
        {
            var image = new Mat(path, LoadImageType.Grayscale);

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
