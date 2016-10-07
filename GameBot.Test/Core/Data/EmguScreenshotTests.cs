using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core.Data;
using NUnit.Framework;
using System;
using System.Drawing;

namespace GameBot.Test.Core.Data
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
        }

        [Test]
        public void ConstructorMat()
        {
            var mat = new Mat("Screenshots/tetris_start.png", LoadImageType.Grayscale);
            Assert.NotNull(mat);

            var width = mat.Width;
            var height = mat.Height;
            var timestamp = TimeSpan.FromSeconds(6);

            IScreenshot screenshot = new EmguScreenshot(mat, timestamp);

            Assert.AreEqual(width, screenshot.Width);
            Assert.AreEqual(height, screenshot.Height);

            Assert.AreEqual(timestamp, screenshot.Timestamp);
        }

        [Test]
        public void ConstructorFile()
        {
            var file = "Screenshots/tetris_start.png";
            var timestamp = TimeSpan.FromSeconds(6);

            IScreenshot screenshot = new EmguScreenshot(file, timestamp);

            Assert.NotNull(screenshot);
            Assert.AreEqual(timestamp, screenshot.Timestamp);
        }

        [Test]
        public void GetPixel()
        {
            var image = Image.FromFile("Screenshots/tetris_start.png");
            Assert.NotNull(image);
            
            var timestamp = TimeSpan.FromSeconds(6);

            IScreenshot screenshot = new EmguScreenshot(image, timestamp);
            
            Assert.AreEqual(0, screenshot.GetPixel(0, 0));
            Assert.AreEqual(255, screenshot.GetPixel(160 - 1, 144 - 1));

            Assert.AreEqual(255, screenshot.GetPixel(25, 20));
            Assert.AreEqual(170, screenshot.GetPixel(25, 25));
            Assert.AreEqual(85, screenshot.GetPixel(25, 33));
            Assert.AreEqual(0, screenshot.GetPixel(25, 44));
        }

        [Test]
        public void GetTileMean()
        {
            var image = Image.FromFile("Screenshots/tetris_play_2.png");
            Assert.NotNull(image);

            var timestamp = TimeSpan.FromSeconds(6);

            IScreenshot screenshot = new EmguScreenshot(image, timestamp);

            for (int i = 0; i < 3; i++)
            {
                // test multiple time
                Assert.AreEqual(41, screenshot.GetTileMean(0, 0));
            }

            Assert.AreEqual(110, screenshot.GetTileMean(1, 0));
            Assert.AreEqual(255, screenshot.GetTileMean(2, 0));

            Assert.AreEqual(92, screenshot.GetTileMean(5, 7));
        }
    }
}
