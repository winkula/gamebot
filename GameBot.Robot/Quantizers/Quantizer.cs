using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Robot.Data;
using System;
using System.Diagnostics;
using System.Drawing;

namespace GameBot.Robot.Quantizers
{
    public class Quantizer : IQuantizer
    {
        private int c = 5;
        private int block = 13;
        private readonly Mat transform;

        public Quantizer()
        {
            Matrix<float> srcKeypoints = new Matrix<float>(new float[,] { { 0 + 100, 0 }, { 640 - 100, 0 }, { 0, 480 }, { 640, 480 } });
            Matrix<float> destKeypoints = new Matrix<float>(new float[,] { { 0, 0 }, { 160, 0 }, { 0, 144 }, { 160, 144 } });

            // calculate transformation matrix
            transform = CvInvoke.GetPerspectiveTransform(srcKeypoints, destKeypoints);
        }

        public IScreenshot Quantize(IImage image, TimeSpan timestamp)
        {
            var sourceImage = image;
            var destImage = new Mat(new Size(160, 144), DepthType.Default, 1);
            var destImageBin = new Mat(new Size(160, 144), DepthType.Default, 1);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // transform
            CvInvoke.WarpPerspective(sourceImage, destImage, transform, new Size(160, 144), Inter.Linear, Warp.Default);

            Debug.WriteLine($"{stopwatch.ElapsedMilliseconds} ms, WarpPerspective");

            // CvInvoke.Imshow("r", destImage);
            // CvInvoke.WaitKey();

            stopwatch.Restart();

            // threshold
            CvInvoke.AdaptiveThreshold(destImage, destImageBin, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, block, c);

            Debug.WriteLine($"{stopwatch.ElapsedMilliseconds} ms, AdaptiveThreshold");

            // CvInvoke.Imshow("r", destImageBin);
            // CvInvoke.WaitKey();

            stopwatch.Restart();
            
            CvInvoke.Imshow("Image_Binarized", destImageBin);

            return new EmguScreenshot(destImageBin, timestamp);
        }
    }
}
