using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using NUnit.Framework;
using System.Diagnostics;

namespace GameBot.Test.Robot.Quantizers
{
    [TestFixture]
    public class BlockQuantizerTests
    {
        private const bool adjust = false;
        
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
