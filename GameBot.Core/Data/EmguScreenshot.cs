using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace GameBot.Core.Data
{
    /// <summary>
    /// Represents a fast implementation for further processing with EmguCV.
    /// </summary>
    public class EmguScreenshot : IScreenshot
    {
        private const int _tileSize = GameBoyConstants.TileSize;

        public Mat Image { get; }
        public Mat OriginalImage { get; set; }
        public int Width { get; }
        public int Height { get; }
        public TimeSpan Timestamp { get; }
        
        public EmguScreenshot(Mat image, TimeSpan timestamp)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            Image = image;

            Width = image.Width;
            Height = image.Height;
            Timestamp = timestamp;
        }

        public EmguScreenshot(string file, TimeSpan timestamp) : this(new Mat(file, LoadImageType.Grayscale), timestamp)
        {
        }

        public byte GetPixel(int x, int y)
        {
            using (var memoryImage = Image.ToImage<Gray, byte>())
            {
                return memoryImage.Data[y, x, 0];
            }
        }

        public byte GetTileMean(int x, int y)
        {
            if (x < 0 || x >= Width / _tileSize) throw new ArgumentException("x is off the screen");
            if (y < 0 || y >= Height / _tileSize) throw new ArgumentException("y is off the screen");

            var roi = new Rectangle(x * _tileSize, y * _tileSize, _tileSize, _tileSize);
            var roiedImage = new Mat(Image, roi);

            var mean = CvInvoke.Mean(roiedImage);
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
