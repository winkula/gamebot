using System;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Test.Extensions;
using Moq;

namespace GameBot.Test
{
    public static class TestHelper
    {
        private static readonly Random _random = new Random();

        public static IScreenshot GetScreenshot(string path, IQuantizer quantizer)
        {
            var image = new Mat(path, LoadImageType.AnyColor);
            var quantized = quantizer.Quantize(image);

            return new EmguScreenshot(quantized, DateTime.Now.Subtract(DateTime.MinValue));
        }

        public static void Show(IScreenshot screenshot)
        {
            CvInvoke.Imshow("Test", screenshot.Image);
            CvInvoke.WaitKey();
        }

        public static void Show(Mat mat)
        {
            CvInvoke.Imshow("Test", mat);
            CvInvoke.WaitKey();
        }

        public static void Save(IScreenshot screenshot, string filename)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), filename);
            screenshot.Image.Save(path);
        }

        public static void Save(Mat mat, string filename)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), filename);
            mat.Save(path);
        }

        public static Board GetRandomBoard(int maxHeight)
        {
            var board = new Board();

            for (int x = 0; x < board.Width - 1; x++)
            {
                var height = _random.Next(0, maxHeight);
                for (int y = 0; y < height; y++)
                {
                    if (_random.NextDouble() < 0.95)
                    {
                        board.Occupy(x, y);
                    }
                }
            }

            return board;
        }

        public static Board BuildBoard(int[] squares)
        {
            var board = new Board();
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    if (squares[board.Width * (board.Height - 1 - y) + (x)] > 0)
                    {
                        board.Occupy(x, y);
                    }
                }
            }
            return board;
        }

        public static Mock<IConfig> GetFakeConfig()
        {
            var configMock = new Mock<IConfig>();

            configMock.ConfigValue("Game.Tetris.Visualize", true);

            configMock.ConfigValue("Game.Tetris.StartLevel", 9);
            configMock.ConfigValue("Game.Tetris.HeartMode", false);
            configMock.ConfigValue("Game.Tetris.StartFromGameOver", false);

            configMock.ConfigValue("Game.Tetris.Multiplayer", false);

            configMock.ConfigValue("Game.Tetris.Check.Enabled", false);

            configMock.ConfigValue("Game.Tetris.Extractor.Samples", 3);
            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdNextPiece", 0.7);
            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdCurrentPiece", 0.7);
            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdMovedPiece", 0.5);

            configMock.ConfigValue("Game.Tetris.Timing.MoreTimeToAnalyze", 50);
            configMock.ConfigValue("Game.Tetris.Timing.LessWaitTimeAfterDrop", 50);
            configMock.ConfigValue("Game.Tetris.Timing.LessFallTimeBeforeDrop", 50);

            configMock.ConfigValue("Emulator.Rom.Path", "Roms/tetris.gb");

            configMock.ConfigValue("Robot.Engine.Mode", "Emulated");

            configMock.ConfigValue("Robot.Camera.Index", 0);
            configMock.ConfigValue("Robot.Camera.RotateImage", false);
            configMock.ConfigValue("Robot.Camera.Noise", false);

            configMock.ConfigCollection("Robot.Quantizer.Transformation.KeyPoints", new[] { 0, 0, 160, 0, 0, 144, 160, 144 });
            configMock.ConfigValue("Robot.Quantizer.Threshold.Constant", 13);
            configMock.ConfigValue("Robot.Quantizer.Threshold.BlockSize", 17);
            configMock.ConfigValue("Robot.Quantizer.Blur", false);

            configMock.ConfigValue("Robot.Actuator.Host", "localhost");
            configMock.ConfigValue("Robot.Actuator.Port", 4223);
            configMock.ConfigValue("Robot.Actuator.UidMaster", "6JKbWn");
            configMock.ConfigValue("Robot.Actuator.UidRelay1", "mTA");
            configMock.ConfigValue("Robot.Actuator.UidRelay2", "mTC");
            configMock.ConfigValue("Robot.Actuator.Hit.Time", 35);
            configMock.ConfigValue("Robot.Actuator.Hit.DelayAfter", 40);

            configMock.ConfigValue("Robot.Ui.LogLevel", "Info");

            return configMock;
        }
    }
}
