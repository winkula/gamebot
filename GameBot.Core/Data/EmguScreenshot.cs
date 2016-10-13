using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GameBot.Core.Data
{
    /// <summary>
    /// Represents a fast implementation for further processing with EmguCV.
    /// </summary>
    public class EmguScreenshot : IScreenshot
    {
        public const int TileSize = GameBoyConstants.TileSize;
        public static Mat Black;

        private readonly IImage image;

        public byte[] Pixels { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TimeSpan Timestamp { get; private set; }

        static EmguScreenshot()
        {
            Black = new Mat(new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight), DepthType.Cv8U, 1);
            Black.SetTo(new MCvScalar(0, 0, 0));
        }

        public EmguScreenshot(IImage image, TimeSpan timestamp)
        {
            this.image = image;

            var mat = image.GetInputArray().GetMat();
            Width = mat.Width;
            Height = mat.Height;
            Pixels = new byte[Width * Height];
            Marshal.Copy(mat.DataPointer, Pixels, 0, Width * Height);
            Timestamp = timestamp;
        }

        public EmguScreenshot(Image image, TimeSpan timestamp) : this(new Image<Gray, byte>(new Bitmap(image)), timestamp)
        {
        }

        public EmguScreenshot(Bitmap bitmap, TimeSpan timestamp) : this(new Image<Gray, byte>(bitmap), timestamp)
        {
        }

        public EmguScreenshot(string file, TimeSpan timestamp) : this(new Mat(file, LoadImageType.Grayscale), timestamp)
        {
        }

        public byte GetPixel(int x, int y)
        {
            return Pixels[y * Width + x];
        }

        public byte[] GetTile(int x, int y)
        {
            var tile = new byte[TileSize * TileSize];
            for (int yIn = 0; yIn < TileSize; yIn++)
            {
                for (int xIn = 0; xIn < TileSize; xIn++)
                {
                    tile[TileSize * yIn + xIn] = GetPixel(TileSize * x + xIn, TileSize * y + yIn);
                }
            }
            return tile;
        }

        public byte GetTileMean(int x, int y)
        {
            // this makes tiles white which are above the board (for easier piece matching)
            // TODO: solve better
            if (y < 0) return 255;

            // TODO: implement faster? (with ROI maybe)
            // TODO: cache mean values for fast multiple lookup

            var mask = Black.Clone();
            var roi = new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize);
            CvInvoke.Rectangle(mask, roi, new MCvScalar(255, 255, 255), -1);

            var mean = CvInvoke.Mean(image, mask);
            return (byte)mean.V0;
        }
    }
}
