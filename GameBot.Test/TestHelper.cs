using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using Moq;

namespace GameBot.Test
{
    public static class TestHelper
    {
        private static Random _random = new Random();

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

            ConfigValue(configMock, "Game.Tetris.Visualize", true);
            ConfigValue(configMock, "Game.Tetris.StartLevel", 9);
            ConfigValue(configMock, "Game.Tetris.StartFromGameOver", false);
            ConfigValue(configMock, "Game.Tetris.Check.Enabled", false);
            ConfigValue(configMock, "Game.Tetris.Check.Samples", 1);
            ConfigValue(configMock, "Game.Tetris.Extractor.Samples", 1);
            ConfigValue(configMock, "Game.Tetris.Extractor.ThresholdNextPiece", 0.7);
            ConfigValue(configMock, "Game.Tetris.Extractor.ThresholdCurrentPiece", 0.7);
            ConfigValue(configMock, "Game.Tetris.Extractor.ThresholdMovedPiece", 0.5);
            ConfigValue(configMock, "Emulator.Rom.Path", "Roms/tetris.gb");
            ConfigValue(configMock, "Robot.Engine.Mode", "Emulated");
            ConfigValue(configMock, "Robot.Camera.Index", 0);
            ConfigValue(configMock, "Robot.Camera.RotateImage", false);
            ConfigValue(configMock, "Robot.Camera.Noise", false);
            ConfigCollection(configMock, "Robot.Quantizer.Transformation.KeyPoints", new [] { 0, 0, 160, 0, 0, 144, 160, 144 });
            ConfigValue(configMock, "Robot.Quantizer.Threshold.Constant", 13);
            ConfigValue(configMock, "Robot.Quantizer.Threshold.BlockSize", 17);
            ConfigValue(configMock, "Robot.Quantizer.Blur", false);
            ConfigValue(configMock, "Robot.Actuator.Host", "localhost");
            ConfigValue(configMock, "Robot.Actuator.Port", 4223);
            ConfigValue(configMock, "Robot.Actuator.UidMaster", "6JKbWn");
            ConfigValue(configMock, "Robot.Actuator.UidRelay1", "mTA");
            ConfigValue(configMock, "Robot.Actuator.UidRelay2", "mTC");

            return configMock;
        }
        
        private static void ConfigValue<TValue>(Mock<IConfig> mock, string key, TValue value)
        {
            mock.Setup(x => x.Read<TValue>(key)).Returns(value);
            mock.Setup(x => x.Read(key, It.IsAny<TValue>())).Returns(value);
        }

        private static void ConfigCollection<TValue>(Mock<IConfig> mock, string key, IEnumerable<TValue> value)
        {
            var list = value.ToList();
            mock.Setup(x => x.ReadCollection<TValue>(key)).Returns(list);
            mock.Setup(x => x.ReadCollection(key, It.IsAny<IEnumerable<TValue>>())).Returns(list);
        }
    }
}
