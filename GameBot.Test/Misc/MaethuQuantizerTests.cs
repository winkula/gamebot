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
using System.Linq;
using System.Text;
using GameBot.Core.Configuration;
using GameBot.Core.Data;
using GameBot.Core.Extensions;
using GameBot.Engine.Physical.Quantizers;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;

namespace GameBot.Test.Misc
{
    //[Ignore]
    [TestFixture]
    public class MaethuQuantizerTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void OpenMorphological()
        {
            var data = ImageTestCaseFactory.Data;
            foreach (var testData in data)
            {
                var quantizer = new Quantizer(new AppSettingsConfig());
                quantizer.ThresholdConstant = 13;
                quantizer.ThresholdBlockSize = 17; 
                quantizer.Calibrate(testData.Keypoints);
                var src = quantizer.Quantize(testData.Image);

                //Image<Gray, byte> src = new Image<Gray, byte>(@"C:\Users\Winkler\Desktop\quantizer_output.png");
                Image<Gray, byte> dst = new Image<Gray, byte>(src.Size.Width, src.Size.Height);
                var kernel = new ConvolutionKernelF(new float[,]
                {
                    { 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1 },
                });

                var sw = new Stopwatch();
                sw.Start();

                CvInvoke.MorphologyEx(src, dst, MorphOp.Open, kernel, new Point(-1, -1), 1, BorderType.Replicate, new MCvScalar(-1));

                sw.Stop();
                _logger.Info($"Time for MorphologyEx: {sw.ElapsedMilliseconds}");

                dst.SaveToDesktop("morpho");
                CvInvoke.Imshow("test", dst);
                CvInvoke.WaitKey();
            }

        }

        [Test]
        public void Roi()
        {
            var mat = new Image<Gray, byte>(160, 144);

            CvInvoke.cvSetImageROI(mat.Ptr, new Rectangle(10, 10, 10, 10));
        }

        [Test]
        public void PieceMatcher()
        {
            // source image
            string path = "Screenshots/white.png";
            var sourceImage = new Mat(path, LoadImageType.Grayscale);
            var screenshot = new EmguScreenshot(sourceImage, TimeSpan.Zero);

            var pieceMatcher = new TemplateMatcher();
            pieceMatcher.GetProbabilityNextPiece(screenshot, Tetrimino.I);
        }

        [Test]
        public void RenameTestData()
        {
            foreach (var record in ImageTestCaseFactory.Data)
            {
                var sb = new StringBuilder();

                if (record.Piece != null)
                {
                    sb.Append($"{record.Piece.Tetrimino}{record.Piece.Orientation:D1}{record.Piece.X:D3}{-record.Piece.Y:D3}");
                }
                else
                {
                    sb.Append(new string('_', 6));
                }

                sb.Append("_");

                if (record.NextPiece.HasValue)
                {
                    sb.Append($"{record.NextPiece.Value}");
                }
                else
                {
                    sb.Append("_");
                }

                sb.Append("_");
                
                sb.Append(new string('_', 2 * 10));

                sb.Append("_");

                sb.Append(string.Join("", record.Keypoints.Select(p => $"{p.X:D3}{p.Y:D3}")));

                // TOXXYY_T_00112233445566778899_000111222333444555666777

                string name = sb.ToString();
                File.Copy($"Images/test{record.ImageKey}.jpg", $"Images/{name}.jpg");
            }
        }

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
                //CannyDilate(img);
                //TemplateMatching(img);
            }
            stopwatch.Stop();
            _logger.Info($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");

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

        /*
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
        }*/
    }
}
