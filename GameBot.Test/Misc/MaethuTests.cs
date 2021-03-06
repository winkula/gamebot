﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using GameBot.Core;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using GameBot.Core.Data;
using GameBot.Core.Extensions;
using GameBot.Core.Quantizers;
using GameBot.Emulation;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;
using GameBot.Game.Tetris.Searching.Heuristics;

namespace GameBot.Test.Misc
{
    [TestFixture]
    public class MaethuTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Ignore]
        [Test]
        public void GenerateImagesForDoc()
        {
            //var testData0500 = TestDataFactory.Data.Single(x => x.ImageKey == "0500");
            //var testData0413 = TestDataFactory.Data.Single(x => x.ImageKey == "0413");

            var image = new Mat(@"C:\Users\Winkler\Desktop\orig.png", LoadImageType.AnyColor);
            var keypoints = new List<Point> {new Point(140, 116), new Point(477, 120), new Point(163, 370), new Point(447, 369)};
            var testData0500 = new { Keypoints = keypoints, Image = image };

            var configMock = TestHelper.GetFakeConfig();
            //var quantizer = new Quantizer(configMock.Object);

            // simple threshold
            IQuantizer quantizer = new SimpleThresholdQuantizer { Threshold = 220 };
            quantizer.Keypoints = testData0500.Keypoints;
            var quantizedImage = quantizer.Quantize(testData0500.Image);
            //TestHelper.Show(quantizedImage);
            TestHelper.Save(quantizedImage, "threshold_simple.png");

            // adaptive threshold
            quantizer = new Quantizer(configMock.Object) { ThresholdBlockSize = 17, ThresholdConstant = 6 };
            quantizer.Keypoints = testData0500.Keypoints;
            quantizedImage = quantizer.Quantize(testData0500.Image);
            //TestHelper.Show(quantizedImage);
            TestHelper.Save(quantizedImage, "threshold_adaptive.png");
            
            // warp
            quantizer = new WarpOnlyQuantizer();
            quantizer.Keypoints = testData0500.Keypoints;
            quantizedImage = quantizer.Quantize(testData0500.Image);
            //TestHelper.Show(quantizedImage);
            TestHelper.Save(quantizedImage, "warp_result.png");

            // morphological
            quantizer = new MorphologyQuantizer(configMock.Object) { ThresholdBlockSize = 17, ThresholdConstant = 6 };
            quantizer.Keypoints = testData0500.Keypoints;
            quantizedImage = quantizer.Quantize(testData0500.Image);
            //TestHelper.Show(quantizedImage);
            TestHelper.Save(quantizedImage, "opening_result.png");
        }
        
        [Ignore]
        [Test]
        public void TestManyScorings()
        {
            var gameStates = Enumerable.Range(0, 22 * 22 * 7 * 22)
                .Select(x => new Board().Random())
                .Select(x => new GameState(x))
                .ToList();

            var heuristic = new YiyuanLeeHeuristic();

            var sw = new Stopwatch();
            sw.Start();
            
            foreach (var gs in gameStates)
            {
                var score = heuristic.Score(gs);
            }

            sw.Stop();
            Debug.WriteLine(sw.ElapsedMilliseconds);
        }

        [Ignore]
        [Test]
        public void GenerateGameOverReferenceImage()
        {
            var configMock = TestHelper.GetFakeConfig();

            var screenshot = TestHelper.GetScreenshot("Screenshots/multiplayer_gameover.png", new MorphologyQuantizer(configMock.Object));
            TestHelper.Save(screenshot, "multiplayer_gameover_ref.png");
        }

        [Ignore]
        [Test]
        public void TestFallSpeedLevel0()
        {
            var romLoader = new RomLoader();
            var rom = romLoader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(rom);

            emulator.Execute(125);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);

            for (int i = 0; i < 14; i++)
            {
                emulator.Show();
                emulator.Execute(53);
            }
        }

        [Ignore]
        [Test]
        public void TestDropSpeedLevel0()
        {
            var romLoader = new RomLoader();
            var rom = romLoader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(rom);

            emulator.Execute(125);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);

            emulator.Press(Button.Down);

            for (int i = 0; i < 14; i++)
            {
                emulator.Show();
                emulator.Execute(3);
            }
        }

        [Ignore]
        [Test]
        public void TestFallSpeedLevel9()
        {
            var romLoader = new RomLoader();
            var rom = romLoader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(rom);

            emulator.Execute(125);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Down);
            emulator.Hit(Button.Start);

            for (int i = 0; i < 14; i++)
            {
                emulator.Show();
                emulator.Execute(11);
            }
        }
        
        [Ignore]
        [Test]
        public void TestHeartModeSpeed()
        {
            var romLoader = new RomLoader();
            var rom = romLoader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(rom);

            emulator.Execute(125);
            emulator.Press(Button.Down);
            emulator.Hit(Button.Start);
            emulator.Release(Button.Down);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Down);
            emulator.Hit(Button.Start);

            for (int i = 0; i < 14; i++)
            {
                emulator.Show();
                emulator.Execute(4);
            }
        }

        [Ignore]
        [Test]
        public void TestDropSpeedLevel9()
        {
            var romLoader = new RomLoader();
            var rom = romLoader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(rom);

            emulator.Execute(125);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Down);
            emulator.Hit(Button.Start);
            
            emulator.Press(Button.Down);

            for (int i = 0; i < 14; i++)
            {
                emulator.Show();
                emulator.Execute(3);
            }
        }

        [Ignore]
        [Test]
        public void TestLineRemoveDuration()
        {
            var romLoader = new RomLoader();
            var rom = romLoader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(rom);

            emulator.Execute(125);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);

            // I
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Press(Button.Down);
            emulator.Execute(3 * 17);
            emulator.Release(Button.Down);
            emulator.Execute(2);
            

            // S
            emulator.Hit(Button.Right);
            emulator.Press(Button.Down);
            emulator.Execute(3 * 17);
            emulator.Release(Button.Down);
            emulator.Execute(2);

            // O
            emulator.Hit(Button.Left);
            emulator.Hit(Button.Left);
            emulator.Hit(Button.Left);
            emulator.Hit(Button.Left);
            emulator.Press(Button.Down);
            emulator.Execute(3 * 17);
            emulator.Release(Button.Down);
            emulator.Execute(2);
            
            // T
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Press(Button.Down);
            emulator.Execute(3 * 17);
            emulator.Release(Button.Down);
            emulator.Execute(2);

            // J
            emulator.Hit(Button.A);
            emulator.Hit(Button.Left);
            emulator.Press(Button.Down);
            emulator.Execute(3 * 15);
            emulator.Release(Button.Down);

            emulator.Show();
            emulator.Execute(93);
            emulator.Show();
        }

        [Ignore]
        [Test]
        public void TestEntryDelayDuration()
        {
            var romLoader = new RomLoader();
            var rom = romLoader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(rom);

            emulator.Execute(125);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);
            
            emulator.Execute(16 * 53); // just landed after this
            emulator.Execute(53); // placed
            emulator.Execute(3);
            emulator.Show();
            return;
        }

        [Ignore]
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

        [Ignore]
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

        [Ignore]
        [Test]
        public void NoiseTests()
        {
            for (int i = 0; i <= 10; i++)
            {
                var test = TestDataFactory.Data.First();
                var noised = test.Image.AddNoise(i / 10.0);

                CvInvoke.Imshow("test", noised);
                CvInvoke.WaitKey();
            }
        }

        [Ignore]
        [Test]
        public void TestMe()
        {
            // source image
            var test = TestDataFactory.Data.First();
            var sourceImage = test.Image;

            var keypoints = new float[,] {
                { test.Keypoints[0].X, test.Keypoints[0].Y },
                { test.Keypoints[1].X, test.Keypoints[1].Y },
                { test.Keypoints[2].X, test.Keypoints[2].Y },
                { test.Keypoints[3].X, test.Keypoints[3].Y }
            };

            // open window
            CvInvoke.NamedWindow("Test");

            var stopwatch = new Stopwatch();

            // destination image (memory allocation), empty
            var img = new Mat(sourceImage.Size, DepthType.Default, 1);

            // calculate transformation matrix
            Matrix<float> srcKeypoints = new Matrix<float>(keypoints);
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
        
        private void GaussAndAdaptive(Mat img)
        {
            CvInvoke.GaussianBlur(img, img, new Size(3, 3), 0.6, 0.6, BorderType.Default);
            CvInvoke.AdaptiveThreshold(img, img, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 5, 5);
        }

        private void GaussContrastAndAdaptive(Mat img)
        {
            CvInvoke.GaussianBlur(img, img, new Size(3, 3), 0.6, 0.6, BorderType.Default);
            ((Mat)img).ConvertTo(img, DepthType.Default, 2, -200);
            CvInvoke.AdaptiveThreshold(img, img, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 5, 5);
        }

        private void Canny(Mat img)
        {
            CvInvoke.Canny(img, img, 200, 900, 5);
        }
    }
}
