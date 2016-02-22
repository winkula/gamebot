using System;
using System.Drawing;

namespace GameBot.Core.DisplayStates
{
    public class DisplayState : IDisplayState
    {
        private const int _width = 160;
        private const int _height = 144;
        private readonly byte[] _pixels;
        private readonly TimeSpan _timestamp;

        public DisplayState(long ticks, byte[] pixels)
        {
            _pixels = pixels;
            _timestamp = new TimeSpan(ticks);
        }

        public DisplayState(long ticks, Image image)
        {
            using (Bitmap bitmap = new Bitmap(image))
            {
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        _pixels[_height * y + x] = (byte)(bitmap.GetPixel(x, y).GetBrightness() * 3.99);
                    }
                }
            }
            _timestamp = new TimeSpan(ticks);
        }

        private byte QuantizePixelValue(float value)
        {
            return (byte)(value * 3.99);
        }

        public byte GetPixel(int x, int y)
        {
            return _pixels[_height * y + x];
        }

        public byte[] GetPixels()
        {
            return _pixels;
        }

        public TimeSpan GetTimestamp()
        {
            return _timestamp;
        }
    }
}
