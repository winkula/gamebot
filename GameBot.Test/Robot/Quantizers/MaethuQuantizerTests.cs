using Emgu.CV;
using Emgu.CV.CvEnum;
using NUnit.Framework;
using System.Diagnostics;
using System.Drawing;

namespace GameBot.Test.Robot.Quantizers
{
    [TestFixture]
    public class MaethuQuantizerTests
    {
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
            Matrix<float> destKeypoints = new Matrix<float>(new float[,] { { 0, 0 }, { 160, 0 }, { 0, 144 }, { 160, 144 } });
            var matrix = CvInvoke.GetPerspectiveTransform(srcKeypoints, destKeypoints);
            
            // transform (unwarp and resize to gameboy screen size 160x144)
            CvInvoke.WarpPerspective(sourceImage, img, matrix, new Size(160, 144), Inter.Linear, Warp.Default);

            // playground....
            stopwatch.Start();
            {
                GaussAndAdaptive(img);
                //GaussContrastAndAdaptive(img);
                //Canny(img);
                //TemplateMatching(img);
            }
            stopwatch.Stop();
            Debug.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");

            // show/save image
            img.Save(@"C:\users\winkler\desktop\output.png");
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

        private void TemplateMatching(IImage img)
        {
            var template = new Mat(@"C:\users\winkler\desktop\block.png", LoadImageType.Grayscale);

            CvInvoke.GaussianBlur(img, img, new Size(3, 3), 0.3, 0.3, BorderType.Default);
            //CvInvoke.AdaptiveThreshold(img, img, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 5, 5);
            CvInvoke.MatchTemplate(img, template, img, TemplateMatchingType.CcoeffNormed);            
        }
    }
}
