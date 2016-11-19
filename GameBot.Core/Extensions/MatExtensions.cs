using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace GameBot.Core.Extensions
{
    public static class MatExtensions
    {
        public static Mat AddNoise(this Mat image, double noiseLevel = 0.75)
        {
            if (noiseLevel < 0 || noiseLevel > 1.0) throw new ArgumentException("noiseLevel must be between 0.0 and 1.0");
            if (noiseLevel == 0) return image;

            var mean = new MCvScalar(0);
            var std = new MCvScalar(255);
            const int gaussSize = 13;

            var output = new Mat();
            var noise = new Mat(image.Size, DepthType.Cv8U, image.NumberOfChannels);

            using (ScalarArray scalarArray1 = new ScalarArray(mean))
            using (ScalarArray scalarArray2 = new ScalarArray(std))
            {
                CvInvoke.Randn(noise, scalarArray1, scalarArray2);
            }
            CvInvoke.GaussianBlur(noise, noise, new Size(gaussSize, gaussSize), 0.0);
            CvInvoke.AddWeighted(image, 1 - noiseLevel, noise, noiseLevel, 0, output, image.Depth);

            return output;
        }
    }
}
