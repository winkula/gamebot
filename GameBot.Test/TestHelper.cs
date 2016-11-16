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
    }
}
