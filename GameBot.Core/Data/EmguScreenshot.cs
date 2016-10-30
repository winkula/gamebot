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
        private const int _tileSize = GameBoyConstants.TileSize;
        private static readonly Mat _black;

        public IImage Image { get; }
        public IImage OriginalImage { get; set; }
        public byte[] Pixels { get; }
        public int Width { get; }
        public int Height { get; }
        public TimeSpan Timestamp { get; }

        static EmguScreenshot()
        {
            _black = new Mat(new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight), DepthType.Cv8U, 1);
            _black.SetTo(new MCvScalar(0, 0, 0));
        }

        public EmguScreenshot(IImage image, TimeSpan timestamp)
        {
            Image = image;

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
            var tile = new byte[_tileSize * _tileSize];
            for (int yIn = 0; yIn < _tileSize; yIn++)
            {
                for (int xIn = 0; xIn < _tileSize; xIn++)
                {
                    tile[_tileSize * yIn + xIn] = GetPixel(_tileSize * x + xIn, _tileSize * y + yIn);
                }
            }
            return tile;
        }

        public byte GetTileMean(int x, int y)
        {
            if (x < 0 || x >= Width / _tileSize) throw new ArgumentException("x is off the screen");
            if (y < 0 || y >= Height / _tileSize) throw new ArgumentException("y is off the screen");

            // TODO: implement faster? (with ROI maybe), cache mean values for fast multiple lookup
            var mask = _black.Clone();
            var roi = new Rectangle(x * _tileSize, y * _tileSize, _tileSize - 1, _tileSize - 1);
            CvInvoke.Rectangle(mask, roi, new MCvScalar(255, 255, 255), -1);

            var mean = CvInvoke.Mean(Image, mask);
            return (byte)mean.V0;
        }

        public override int GetHashCode()
        {
            return Timestamp.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            IScreenshot other = obj as IScreenshot;
            if (other != null)
            {
                return Timestamp.Equals(other.Timestamp);
            }
            return false;
        }
    }
}
