using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;

namespace GameBot.Core.Quantizers
{
    public class PassthroughQuantizer : CalibrateableQuantizer
    {
        public PassthroughQuantizer()
        {
            Keypoints = new List<Point>
            {
                new Point(0,0 ),
                new Point(GameBoyConstants.ScreenWidth, 0),
                new Point(0, GameBoyConstants.ScreenHeight),
                new Point( GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight)
            };
        }

        public override Mat Quantize(Mat image)
        {
            return image;
        }
    }
}
