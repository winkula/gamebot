﻿using Emgu.CV;
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
using GameBot.Core.Quantizers;
using GameBot.Engine.Physical.Quantizers;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;

namespace GameBot.Test.Misc
{
    [Ignore]
    [TestFixture]
    public class MaethuQuantizerTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void TestMatRoi()
        {
            const int count = 100;

            var sw = new Stopwatch();
            sw.Start();

            var src = new Mat("Screenshots/tetris_play_1.png", LoadImageType.Grayscale);
            var bin = new Mat();
            var mor = new Mat();

            sw.Stop();
            _logger.Info($"Load: {sw.ElapsedMilliseconds}");
            sw.Restart();

            for (int i = 0; i < count; i++)
            {
                CvInvoke.AdaptiveThreshold(src, bin, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 17, 13);
            }

            sw.Stop();
            _logger.Info($"Threshold: {sw.ElapsedMilliseconds}");
            sw.Restart();

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

            for (int i = 0; i < count; i++)
            {
                CvInvoke.MorphologyEx(bin, mor, MorphOp.Open, kernel, new Point(-1, -1), 1, BorderType.Replicate, new MCvScalar(-1));
            }

            sw.Stop();
            _logger.Info($"MorphologyEx: {sw.ElapsedMilliseconds}");
            sw.Restart();

            for (int i = 0; i < count; i++)
            {
                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 10; y++)
                    {
                        var roi = new Mat(mor, new Rectangle(8 * x, 8 * y, 8, 8));
                        var mean = CvInvoke.Mean(roi);
                    }
                }
            }

            sw.Stop();
            _logger.Info($"Mean: {sw.ElapsedMilliseconds}");
            sw.Restart();

            sw.Stop();
        }

        [Test]
        public void OpenMorphological()
        {
            var data = ImageTestCaseFactory.Data;
            foreach (var testData in data)
            {
                var quantizer = new Quantizer(new AppSettingsConfig());
                quantizer.ThresholdConstant = 13;
                quantizer.ThresholdBlockSize = 17;
                quantizer.Keypoints = testData.Keypoints;
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
            var screenshot = new EmguScreenshot(sourceImage, DateTime.Now.Subtract(DateTime.MinValue));

            var pieceMatcher = new TemplateMatcher();
            pieceMatcher.GetProbabilityNextPiece(screenshot, Tetrimino.I);
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
            CvInvoke.Canny(img, img, 200, 900, 5);
        }
    }
}
