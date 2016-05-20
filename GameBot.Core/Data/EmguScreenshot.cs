using Emgu.CV;
using System;
using System.Runtime.InteropServices;

namespace GameBot.Core.Data
{
    /// <summary>
    /// Represents a fast implementation for further processing with EmguCV.
    /// </summary>
    public class EmguScreenshot : IScreenshot
    {
        public const int TileSize = 8;

        private readonly IImage image;
        private readonly byte[] bytes;
        public int[] Pixels { get { throw new NotImplementedException(); } }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TimeSpan Timestamp { get; private set; }

        public EmguScreenshot(IImage image, TimeSpan timestamp)
        {
            this.image = image;

            var mat = image.GetInputArray().GetMat();
            Width = mat.Width;
            Height = mat.Height;
            bytes = new byte[Width * Height];
            Marshal.Copy(mat.DataPointer, bytes, 0, Width * Height);
            Timestamp = timestamp;
        }

        public int GetPixel(int x, int y)
        {
            var byteValue = bytes[y * Width + x];
            return QuantizePixelValue(byteValue);
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
