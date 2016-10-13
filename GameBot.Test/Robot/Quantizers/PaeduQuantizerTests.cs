using Emgu.CV;
using Emgu.CV.CvEnum;
using NLog;
using NUnit.Framework;
using System.Diagnostics;
using System.Drawing;

namespace GameBot.Test.Robot.Quantizers
{
    [TestFixture]
    public class PaeduQuantizerTests
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void TestMe()
        {
            // TODO: test cann edge detector or other sophisticated filters
            // http://docs.opencv.org/2.4/doc/tutorials/imgproc/imgtrans/canny_detector/canny_detector.html
            // http://www.emgu.com/wiki/index.php/Documentation

            // source image
            string path = "Images/tetris_1.jpg";
            var sourceImage = new Mat(path, LoadImageType.Grayscale);

            // open window
            CvInvoke.NamedWindow("Test");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // destination image (memory allocation), empty
            var destImage = new Mat(sourceImage.Size, DepthType.Default, 1);

            // calculate transformation matrix
            Matrix<float> srcKeypoints = new Matrix<float>(new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } });
            Matrix<float> destKeypoints = new Matrix<float>(new float[,] { { 0, 0 }, { 160, 0 }, { 0, 144 }, { 160, 144 } });
            var matrix = CvInvoke.GetPerspectiveTransform(srcKeypoints, destKeypoints);

            // transform (unwarp and resize to gameboy screen size 160x144)
            CvInvoke.WarpPerspective(sourceImage, destImage, matrix, new Size(160, 144), Inter.Linear, Warp.Default);

            // playground....




            stopwatch.Stop();
            logger.Info($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");

            // show/save image
            //destImage.Save(@"C:\...");
            CvInvoke.Imshow("Test", destImage);
            CvInvoke.WaitKey(0);
        }
    }
}
