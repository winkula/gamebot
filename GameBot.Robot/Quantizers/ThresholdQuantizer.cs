using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using NLog;
using System.Diagnostics;
using System.Drawing;

namespace GameBot.Robot.Quantizers
{
    public class ThresholdQuantizer : IQuantizer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private bool adjust;
        private int threshold = 50;
        private float[,] keypoints = new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } };

        public ThresholdQuantizer()
        {
        }

        public ThresholdQuantizer(bool adjust, float[,] keypoints, int threshold)
        {
            this.adjust = adjust;
            this.keypoints = keypoints;
            this.threshold = threshold;
        }

        /*
        public IScreenshot Quantize(IImage image, TimeSpan timestamp)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Image<Gray, Byte> destImage = new Image<Gray, Byte>(160, 144);
            Image<Gray, Byte> destImageBin = new Image<Gray, Byte>(160, 144);

            Matrix<float> sourceMat = new Matrix<float>(keypoints);
            Matrix<float> destMat = new Matrix<float>(new float[,] { { 0, 0 }, { 160, 0 }, { 0, 144 }, { 160, 144 } });

            var transform = CvInvoke.GetPerspectiveTransform(image, destMat);

            logger.Info($"{stopwatch.ElapsedMilliseconds} ms, GetPerspectiveTransform");
            stopwatch.Restart();

            // transform
            CvInvoke.WarpPerspective(sourceImage, destImageMat, transform, new Size(160, 144), Inter.Linear, Warp.Default);


            logger.Info($"{stopwatch.ElapsedMilliseconds} ms, Threshold");
            stopwatch.Restart();

            CvInvoke.Threshold(destImage, destImageBin, threshold, 255, ThresholdType.Binary);

            logger.Info($"{stopwatch.ElapsedMilliseconds} ms, Threshold");
            stopwatch.Restart();

            while (adjust)
            {
                CvInvoke.Threshold(destImage, destImageBin, threshold, 255, ThresholdType.Binary);

                CvInvoke.NamedWindow("Test");
                CvInvoke.Imshow("Test", destImageBin);

                int key = CvInvoke.WaitKey();
                if (key == 2424832) threshold++;
                if (key == 2555904) threshold--;
                if (key == 27) break;

                logger.Info("Threshold: " + threshold);
            }

            var screenshot = new Screenshot(destImageBin.ToBitmap(), new TimeSpan());
            return screenshot;
        }
        */
        public IImage Quantize(IImage image)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var sourceImage = image;
            var destImage = new Mat(sourceImage.Size, DepthType.Default, 1);
            var destImageBin = new Mat(sourceImage.Size, DepthType.Default, 1);
            Matrix<float> srcKeypoints = new Matrix<float>(keypoints);
            Matrix<float> destKeypoints = new Matrix<float>(new float[,] { { 0, 0 }, { 160, 0 }, { 0, 144 }, { 160, 144 } });

            // calculate transformation matrix
            var transform = CvInvoke.GetPerspectiveTransform(srcKeypoints, destKeypoints);

            logger.Info($"{stopwatch.ElapsedMilliseconds} ms, GetPerspectiveTransform");
            stopwatch.Restart();

            // transform
            CvInvoke.WarpPerspective(sourceImage, destImage, transform, new Size(160, 144), Inter.Linear, Warp.Default);

            logger.Info($"{stopwatch.ElapsedMilliseconds} ms, WarpPerspective");
            stopwatch.Restart();

            // threshold
            CvInvoke.Threshold(destImage, destImageBin, threshold, 255, ThresholdType.Binary);

            logger.Info($"{stopwatch.ElapsedMilliseconds} ms, Threshold");
            stopwatch.Restart();

            while (adjust)
            {
                CvInvoke.Threshold(destImage, destImageBin, threshold, 255, ThresholdType.Binary);

                CvInvoke.NamedWindow("Test");
                CvInvoke.Imshow("Test", destImageBin);

                int key = CvInvoke.WaitKey();
                if (key == 2424832) threshold++;
                if (key == 2555904) threshold--;
                if (key == 27) break;

                logger.Info("Threshold: " + threshold);
            }

            return destImageBin;
        }
    }
}
