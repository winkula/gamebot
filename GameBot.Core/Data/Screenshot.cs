using System;
using System.Drawing;

namespace GameBot.Core.Data
{
    public class Screenshot : IScreenshot
    {
        public byte[] Pixels { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TimeSpan Timestamp { get; private set; }

        public Screenshot(byte[] pixels, int width, int height, TimeSpan timestamp)
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
                Pixels = new byte[image.Width * image.Height];
                Width = image.Width;
                Height = image.Height;
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        byte value = (byte)(bitmap.GetPixel(x, y).GetBrightness() * 3.99);
                        SetPixel(x, y, value);
                    }
                }
            }
            Timestamp = timestamp;
        }

        private byte QuantizePixelValue(float value)
        {
            return (byte)(value * 3.99);
        }

        public byte GetPixel(int x, int y)
        {
            return Pixels[Width * y + x];
        }

        private void SetPixel(int x, int y, byte value)
        {
            Pixels[Width * y + x] = value;
        }

        public byte[] GetTile(int x, int y)
        {
            const int size = 8;
            var tile = new byte[size * size];
            for (int yIn = 0; yIn < size; yIn++)
            {
                for (int xIn = 0; xIn < size; xIn++)
                {
                    tile[size * yIn + xIn] = GetPixel(size * y + yIn, size * x + xIn);
                }
            }
            return tile;
        }
    }
}
