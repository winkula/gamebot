using GameBot.Core.Data;
using GameBot.Emulation;
using GameBot.Robot.Sensors;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GameBot.Test
{
    [TestFixture]
    class QuantizerTests
    {
        [Test]
        public void Quantize()
        {
            var quantizer = new Quantizer();
            var image = Image.FromFile("Images/tetris_1.jpg");

            Assert.NotNull(image);

            var w = new Stopwatch();
            w.Start();
            var screenshot = quantizer.Quantize(image);
            w.Stop();

            Debug.Write(w.ElapsedMilliseconds);

            Assert.NotNull(screenshot);
        }
    }
}
