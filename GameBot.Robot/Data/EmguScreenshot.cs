using Emgu.CV;
using GameBot.Core.Data;
using System;
using System.Runtime.InteropServices;

namespace GameBot.Robot.Data
{
    // TODO: cleanup
    public class EmguScreenshot : IScreenshot
    {
        public const int TileSize = 8;

        private readonly IImage image;
        private readonly byte[] bytes;
        //private readonly Bitmap bitmap;
        public int[] Pixels { get { throw new NotImplementedException(); } }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TimeSpan Timestamp { get; private set; }

        public EmguScreenshot(IImage image, TimeSpan timestamp)
        {
            this.image = image;
            //this.bitmap = image.Bitmap;

            var mat = image.GetInputArray().GetMat();
            Width = mat.Width;
            Height = mat.Height;
            bytes = new byte[Width * Height];
            Marshal.Copy(mat.DataPointer, bytes, 0, Width * Height);
            Timestamp = timestamp;
        }
        /*
        public int[] Pixels
        {
            get { return Image.Data.Cast<int>().ToArray(); }
        }*/

            /*
        private int GetPixelInternal(Bitmap image, int x, int y)
        {
            byte[] destination = new byte[1];
            Marshal.Copy(image.DataPointer + (y * image.Cols + x) * image.ElementSize, destination, 0, 1);
            return destination[0];
            //Marshal.Copy(Image.DataPointer, destination, (y * Image.Width + x), 1);
            //return QuantizePixelValue(destination[0]);
            //return QuantizePixelValue(Pixels[y * Width + x]);
        }*/

        public int GetPixel(int x, int y)
        {
            var byteValue = bytes[y * Width + x];
            return QuantizePixelValue(byteValue);
        }

        private int QuantizePixelValue(int byteValue)
        {
            return (255 - byteValue) / 64;
        }

        private int QuantizePixelValue(float brightness)
        {
            return (byte)((1 - brightness) * 3.99);
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
