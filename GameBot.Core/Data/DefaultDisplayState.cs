using System;
using System.Drawing;

namespace GameBot.Core.Data
{
    public class DefaultDisplayState : IDisplayState
    {
        private const int width = 160;
        private const int height = 144;
        private readonly byte[] pixels;
        private readonly TimeSpan timestamp;

        public DefaultDisplayState(long ticks, byte[] pixels)
        {
            this.pixels = pixels;
            this.timestamp = new TimeSpan(ticks);
        }

        public DefaultDisplayState(long ticks, Image image)
        {
            using (Bitmap bitmap = new Bitmap(image))
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixels[height * y + x] = (byte)(bitmap.GetPixel(x, y).GetBrightness() * 3.99);
                    }
                }
            }
            timestamp = new TimeSpan(ticks);
        }

        private byte QuantizePixelValue(float value)
        {
            return (byte)(value * 3.99);
        }

        public byte GetPixel(int x, int y)
        {
            return pixels[height * y + x];
        }

        public byte[] GetTile(int x, int y)
        {
            throw new NotImplementedException();
        }

        public byte[] GetPixels()
        {
            return pixels;
        }

        public TimeSpan GetTimestamp()
        {
            return timestamp;
        }
    }
}
