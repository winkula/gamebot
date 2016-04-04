using System;
using System.Drawing;

namespace GameBot.Core.Data
{
    public class Screenshot : IScreenshot
    {
        public const int TileSize = 8;

        public int[] Pixels { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TimeSpan Timestamp { get; private set; }

        public Screenshot(int[] pixels, int width, int height, TimeSpan timestamp)
        {
            Pixels = pixels;
            Width = width;
            Height = height;
            Timestamp = timestamp;
        }

        public Screenshot(Image image, TimeSpan timestamp)
        {
            using (Bitmap bitmap = new Bitmap(image))
            {
                Pixels = new int[image.Width * image.Height];
                Width = image.Width;
                Height = image.Height;
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        var color = bitmap.GetPixel(x, y);
                        int value = QuantizePixelValue(color.GetBrightness());
                        SetPixel(x, y, value);
                    }
                }
            }
            Timestamp = timestamp;
        }

        private int QuantizePixelValue(float brightness)
        {
            return (byte)((1 - brightness) * 3.99);
        }

        public int GetPixel(int x, int y)
        {
            return Pixels[Width * y + x];
        }

        private void SetPixel(int x, int y, int value)
        {
            Pixels[Width * y + x] = value;
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
