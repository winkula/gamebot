using Emgu.CV;
using Emgu.CV.Structure;
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
        public void ConstructorImage()
        {
            var image = Image.FromFile("Screenshots/tetris_start.png");
            Assert.NotNull(image);
            
            var width = image.Width;
            var height = image.Height;
            var timestamp = TimeSpan.FromSeconds(6);
            
            IScreenshot screenshot = new EmguScreenshot(image, timestamp);
            
            Assert.AreEqual(width, screenshot.Width);
            Assert.AreEqual(height, screenshot.Height);

            Assert.AreEqual(timestamp, screenshot.Timestamp);

            Assert.AreEqual(0, screenshot.GetPixel(0, 0));
            Assert.AreEqual(255, screenshot.GetPixel(160 - 1, 144 - 1));

            Assert.AreEqual(255, screenshot.GetPixel(25, 20));
            Assert.AreEqual(170, screenshot.GetPixel(25, 25));
            Assert.AreEqual(85, screenshot.GetPixel(25, 33));
            Assert.AreEqual(0, screenshot.GetPixel(25, 44));
        }
    }
}
