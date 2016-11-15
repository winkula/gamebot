using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using GameBot.Core.Data;

namespace GameBot.Test
{
    public static class TestHelper
    {
        public static IScreenshot GetScreenshot(string path, IQuantizer quantizer)
        {
            var image = new Mat(path, LoadImageType.AnyColor);
            var quantized = quantizer.Quantize(image);

            return new EmguScreenshot(quantized, DateTime.Now.Subtract(DateTime.MinValue));
        }

        public static void Show(IScreenshot screenshot)
        {
            CvInvoke.Imshow("Test", screenshot.Image);
            CvInvoke.WaitKey();
        }
    }
}
