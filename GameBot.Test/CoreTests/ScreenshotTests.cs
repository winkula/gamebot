using GameBot.Core.Data;
using NUnit.Framework;
using System;
using System.Drawing;

namespace GameBot.Test
{
    [TestFixture]
    public class ScreenshotTests
    {
        [Test]
        public void ConstructorPixels()
        {
            var pixels = new int[] {
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 3, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0,
                0, 0, 0, 0, 0, 0, 0, 2
            };
            var width = 8;
            var height = 7;
            var timestamp = TimeSpan.FromSeconds(6);

            IScreenshot screenshot = new Screenshot(pixels, width, height, timestamp);

            Assert.AreEqual(pixels, screenshot.Pixels);

            Assert.AreEqual(width, screenshot.Width);
            Assert.AreEqual(height, screenshot.Height);

            Assert.AreEqual(timestamp, screenshot.Timestamp);

            Assert.AreEqual(0, screenshot.GetPixel(0, 0));
            Assert.AreEqual(1, screenshot.GetPixel(6, 5));
            Assert.AreEqual(2, screenshot.GetPixel(7, 6));
            Assert.AreEqual(3, screenshot.GetPixel(1, 2));
        }

        [Test]
        public void ConstructorImage()
        {
            var image = Image.FromFile("Screenshots/tetris_start.png");
            Assert.NotNull(image);
            
            var width = image.Width;
            var height = image.Height;
            var timestamp = TimeSpan.FromSeconds(6);

            IScreenshot screenshot = new Screenshot(image, timestamp);
            
            Assert.AreEqual(width, screenshot.Width);
            Assert.AreEqual(height, screenshot.Height);

            Assert.AreEqual(timestamp, screenshot.Timestamp);

            Assert.AreEqual(0, screenshot.GetPixel(0, 0));
            Assert.AreEqual(1, screenshot.GetPixel(7, 12));
            Assert.AreEqual(2, screenshot.GetPixel(3, 9));
            Assert.AreEqual(3, screenshot.GetPixel(3, 8));
        }
    }
}
