using Emgu.CV;
using GameBot.Core.Data;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace GameBot.Robot.Data
{
    public class EmguScreenshot : IScreenshot
    {
        public const int TileSize = 8;

        private readonly Mat Image;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TimeSpan Timestamp { get; private set; }

        public EmguScreenshot(Mat image, TimeSpan timestamp)
        {
            Image = image;
            Timestamp = timestamp;
        }

        public int[] Pixels
        {
            get { return Image.Data.Cast<int>().ToArray(); }
        }
        
        public int GetPixel(int x, int y)
        {
            byte[] value = new byte[1];
            Marshal.Copy(Image.DataPointer + (y * Image.Cols + x) * Image.ElementSize, value, 0, 1);
            return QuantizePixelValue(value[0]);
        }

        private int QuantizePixelValue(int byteValue)
        {
            return (255 - byteValue) / 64;
        }

        public int[] GetTile(int x, int y)
        {
            var tile = new int[TileSize * TileSize];
            for (int yIn = 0; yIn < TileSize; yIn++)
            {
                for (int xIn = 0; xIn < TileSize; xIn++)
                {
                    tile[TileSize * yIn + xIn] = GetPixel(TileSize * x + xIn, TileSize * y + yIn);
                }
            }
            return tile;
        }
    }
}
