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
            var test1 = TestDataFactory.Data.First(x => x.ImageKey == "0102");

            string path = $"Images/test{test1.ImageKey}.jpg";
            var keypoints = new float[,] {
                { test1.Keypoints[0].X, test1.Keypoints[0].Y },
                { test1.Keypoints[1].X, test1.Keypoints[1].Y },
                { test1.Keypoints[2].X, test1.Keypoints[2].Y },
                { test1.Keypoints[3].X, test1.Keypoints[3].Y }
            };

            var quantizer = new AdaptiveThresholdQuantizer(_adjust, keypoints, 17, 23);
            //var quantizer = new AdaptiveThresholdQuantizer(_adjust, keypoints, 5, 13);
            //var qunatizer = new ThresholdQuantizer(_adjust, keypoints, 135);

            TestQuantizer(path, quantizer);
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
