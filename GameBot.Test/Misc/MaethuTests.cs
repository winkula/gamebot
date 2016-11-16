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
using GameBot.Core.Quantizers;
using GameBot.Emulation;
using GameBot.Engine.Physical.Quantizers;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;

namespace GameBot.Test.Misc
{
    //[Ignore]
    [TestFixture]
    public class MaethuTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void TestFallSpeedLevel0()
        {
            var romLoader = new RomLoader();
            var rom = romLoader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(rom);

            emulator.ExecuteFrames(125);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);

            for (int i = 0; i < 14; i++)
            {
                emulator.Show();
                emulator.ExecuteFrames(53);
            }
        }

        [Test]
        public void TestDropSpeedLevel0()
        {
            var romLoader = new RomLoader();
            var rom = romLoader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(rom);

            emulator.ExecuteFrames(125);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);

            emulator.Press(Button.Down);

            for (int i = 0; i < 14; i++)
            {
                emulator.Show();
                emulator.ExecuteFrames(3);
            }
        }

        [Test]
        public void TestFallSpeedLevel9()
        {
            var romLoader = new RomLoader();
            var rom = romLoader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(rom);

            emulator.ExecuteFrames(125);
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
                emulator.ExecuteFrames(11);
            }
        }

        [Test]
        public void TestDropSpeedLevel9()
        {
            var romLoader = new RomLoader();
            var rom = romLoader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(rom);

            emulator.ExecuteFrames(125);
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
                emulator.ExecuteFrames(3);
            }
        }

        [Test]
        public void TestLineRemoveDuration()
        {
            var romLoader = new RomLoader();
            var rom = romLoader.Load("Roms/tetris.gb");

            var emulator = new Emulator();
            emulator.Load(rom);

            emulator.ExecuteFrames(125);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);
            emulator.Hit(Button.Start);

            // I
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Press(Button.Down);
            emulator.ExecuteFrames(3 * 17);
            emulator.Release(Button.Down);
            emulator.ExecuteFrames(2);
            

            // S
            emulator.Hit(Button.Right);
            emulator.Press(Button.Down);
            emulator.ExecuteFrames(3 * 17);
            emulator.Release(Button.Down);
            emulator.ExecuteFrames(2);

            // O
            emulator.Hit(Button.Left);
            emulator.Hit(Button.Left);
            emulator.Hit(Button.Left);
            emulator.Hit(Button.Left);
            emulator.Press(Button.Down);
            emulator.ExecuteFrames(3 * 17);
            emulator.Release(Button.Down);
            emulator.ExecuteFrames(2);
            
            // T
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Hit(Button.Right);
            emulator.Press(Button.Down);
            emulator.ExecuteFrames(3 * 17);
            emulator.Release(Button.Down);
            emulator.ExecuteFrames(2);

            // J
            emulator.Hit(Button.A);
            emulator.Hit(Button.Left);
            emulator.Press(Button.Down);
            emulator.ExecuteFrames(3 * 15);
            emulator.Release(Button.Down);

            emulator.Show();
            emulator.ExecuteFrames(93);
            emulator.Show();
        }

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
