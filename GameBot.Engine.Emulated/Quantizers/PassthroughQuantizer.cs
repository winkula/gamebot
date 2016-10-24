using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using GameBot.Core;

namespace GameBot.Engine.Emulated.Quantizers
{
    public class PassthroughQuantizer : ICalibrateableQuantizer
    {
        public IEnumerable<Point> Keypoints { get; private set; } = new List<Point> { new Point(), new Point(), new Point(), new Point() };

        public IImage Quantize(IImage image)
        {
            return image;
        }
        public void Calibrate(IEnumerable<Point> keypoints)
        {
            Keypoints = keypoints.ToList();
        }
    }
}
