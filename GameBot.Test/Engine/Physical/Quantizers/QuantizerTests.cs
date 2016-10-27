using System.Diagnostics;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using GameBot.Engine.Physical.Quantizers;
using NUnit.Framework;

namespace GameBot.Test.Engine.Physical.Quantizers
{
    [TestFixture]
    public class QuantizerTests
    {
        private const bool _adjust = false;

        [Test]
        public void TestQuantizerFromTestData()
        {
            var test1 = ImageTestCaseFactory.Data.First(x => x.ImageKey == "0102");

            string path = $"Images/test{test1.ImageKey}.jpg";
            var keypoints = new float[,] {
                { test1.Keypoints[0].X, test1.Keypoints[0].Y },
                { test1.Keypoints[1].X, test1.Keypoints[1].Y },
                { test1.Keypoints[2].X, test1.Keypoints[2].Y },
                { test1.Keypoints[3].X, test1.Keypoints[3].Y }
            };

            TestQuantizer(path, new AdaptiveThresholdQuantizer(_adjust, keypoints, 17, 23));
        }

        [Test]
        public void UnwarpAndAdaptiveThreshold1()
        {
            string path = "Images/tetris_1.jpg";
            var keypoints = new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } };

            TestQuantizer(path, new AdaptiveThresholdQuantizer(_adjust, keypoints, 5, 13));
        }

        [Test]
        public void UnwarpAndThreshold1()
        {
            string path = "Images/tetris_1.jpg";
            var keypoints = new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } };

            TestQuantizer(path, new ThresholdQuantizer(_adjust, keypoints, 135));
        }

        [Test]
        public void UnwarpAndAdaptiveThreshold2()
        {
            string path = "Images/tetris_2.jpg";
            var keypoints = new float[,] { { 321, 1677 }, { 2484, 1722 }, { 48, 3740 }, { 2826, 3758 } };

            TestQuantizer(path, new AdaptiveThresholdQuantizer(_adjust, keypoints, 5, 13));
        }

        [Test]
        public void UnwarpAndThreshold2()
        {
            string path = "Images/tetris_2.jpg";
            var keypoints = new float[,] { { 321, 1677 }, { 2484, 1722 }, { 48, 3740 }, { 2826, 3758 } };

            TestQuantizer(path, new ThresholdQuantizer(_adjust, keypoints, 50));
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
