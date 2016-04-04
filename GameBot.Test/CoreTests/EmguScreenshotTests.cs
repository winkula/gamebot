using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Robot.Data;
using NUnit.Framework;
using System;

namespace GameBot.Test
{
    [TestFixture]
    public class EmguScreenshotTests
    {
        [Test]
        public void ConstructorImage()
        {
            var mat = new Mat("Screenshots/tetris_start.png", LoadImageType.Grayscale);
            Assert.NotNull(mat);

            var timestamp = TimeSpan.FromSeconds(6);

            var screenshot = new EmguScreenshot(mat, timestamp);

            Assert.AreEqual(timestamp, screenshot.Timestamp);

            Assert.AreEqual(3, screenshot.GetPixel(0, 0));
            Assert.AreEqual(2, screenshot.GetPixel(7, 12));
            Assert.AreEqual(1, screenshot.GetPixel(3, 9));
            Assert.AreEqual(0, screenshot.GetPixel(3, 8));
        }
    }
}
