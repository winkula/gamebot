using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using GameBot.Core;
using NLog;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace GameBot.Test.Misc
{
    [Ignore]
    [TestFixture]
    public class MaethuQuantizerTests
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void TestMe()
        {
            // source image
            string path = "Images/tetris_1.jpg";
            var sourceImage = new Mat(path, LoadImageType.Grayscale);

            // open window
            CvInvoke.NamedWindow("Test");

            var stopwatch = new Stopwatch();

            // destination image (memory allocation), empty
            var img = new Mat(sourceImage.Size, DepthType.Default, 1);

            // calculate transformation matrix
            Matrix<float> srcKeypoints = new Matrix<float>(new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } });
            Matrix<float> destKeypoints = new Matrix<float>(new float[,] { { 0, 0 }, { GameBoyConstants.ScreenWidth, 0 }, { 0, GameBoyConstants.ScreenHeight }, { GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight } });
            var matrix = CvInvoke.GetPerspectiveTransform(srcKeypoints, destKeypoints);

            // transform (unwarp and resize to gameboy screen size 160x144)
            CvInvoke.WarpPerspective(sourceImage, img, matrix, new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight), Inter.Linear, Warp.Default);

            // playground....
            stopwatch.Start();
            {
                //GaussAndAdaptive(img);
                //GaussContrastAndAdaptive(img);
                //Canny(img);
                CannyDilate(img);
                //TemplateMatching(img);
            }
            stopwatch.Stop();
            logger.Info($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");

            // show/save 
            string outputFilename = "output.png";
            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), outputFilename);
            img.Save(outputPath);
            CvInvoke.Imshow("Test", img);
            CvInvoke.WaitKey(0);
        }

        private void GaussAndAdaptive(IImage img)
        {
            CvInvoke.GaussianBlur(img, img, new Size(3, 3), 0.6, 0.6, BorderType.Default);
            CvInvoke.AdaptiveThreshold(img, img, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 5, 5);
        }

        private void GaussContrastAndAdaptive(IImage img)
        {
            CvInvoke.GaussianBlur(img, img, new Size(3, 3), 0.6, 0.6, BorderType.Default);
            ((Mat)img).ConvertTo(img, DepthType.Default, 2, -200);
            CvInvoke.AdaptiveThreshold(img, img, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 5, 5);
        }

        private void Canny(IImage img)
        {
            //CvInvoke.GaussianBlur(img, img, new Size(3, 3), 0.6, 0.6, BorderType.Default);
            CvInvoke.Canny(img, img, 200, 900, 5);
        }

        private void CannyDilate(IImage img)
        {
            var dilate = new Image<Gray, byte>(TestImages.dilate);

            //CvInvoke.GaussianBlur(img, img, new Size(3, 3), 0.6, 0.6, BorderType.Default);
            CvInvoke.Canny(img, img, 70, 70, 3);
            CvInvoke.Dilate(img, img, dilate, new Point(1, 1), 1, BorderType.Default, new MCvScalar(1));
            //CvInvoke.Erode(img, img, dilate, new Point(1, 1), 1, BorderType.Default, new Emgu.CV.Structure.MCvScalar(1));

            //var template = new Mat(@"C:\users\winkler\desktop\block.png", LoadImageType.Grayscale);
            //CvInvoke.MatchTemplate(img, template, img, TemplateMatchingType.CcoeffNormed);
        }

        private void TemplateMatching(IImage img)
        {
            var template = new Image<Gray, byte>(TestImages.block);

            CvInvoke.GaussianBlur(img, img, new Size(3, 3), 0.3, 0.3, BorderType.Default);
            //CvInvoke.AdaptiveThreshold(img, img, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 5, 5);
            CvInvoke.MatchTemplate(img, template, img, TemplateMatchingType.CcoeffNormed);
        }
    }
}
