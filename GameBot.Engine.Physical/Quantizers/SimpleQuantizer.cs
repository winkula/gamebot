using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameBot.Core.Quantizers;

namespace GameBot.Engine.Physical.Quantizers
{
    public class SimpleQuantizer : CalibrateableQuantizer
    {
        public SimpleQuantizer(IConfig config)
        {
            var keypoints = config.ReadCollection("Robot.Quantizer.Transformation.KeyPoints", new[] { 0 + 100, 0, 640 - 100, 0, 0, 480, 640, 480 }).ToList();
            if (keypoints.Count != 8) throw new ArgumentException("Illegal value for config 'Robot.Quantizer.Transformation.KeyPoints'.");
            
            // precalculate transformation matrix
            Keypoints = new List<Point> { new Point(keypoints[0], keypoints[1]), new Point(keypoints[2], keypoints[3]), new Point(keypoints[4], keypoints[5]), new Point(keypoints[6], keypoints[7]) };
        }

        public override IImage Quantize(IImage image)
        {
            // convert to gray values
            var imageGray = new Mat();
            CvInvoke.CvtColor(image, imageGray, ColorConversion.Rgb2Gray);

            // transform
            var imageWarped = new Mat(new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight), DepthType.Default, 1);
            CvInvoke.WarpPerspective(imageGray, imageWarped, Transform, new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight));
            
            return imageWarped;
        }
    }
}
