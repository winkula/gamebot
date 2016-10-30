using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using GameBot.Core;

namespace GameBot.Engine.Emulated.Quantizers
{
    public class PassthroughQuantizer : IQuantizer
    {
        public IEnumerable<Point> Keypoints { get; set; }

        public PassthroughQuantizer()
        {
            Keypoints = new List<Point> { new Point(), new Point(), new Point(), new Point() };
        }

        public IImage Quantize(IImage image)
        {
            return image;
        }
    }
}
