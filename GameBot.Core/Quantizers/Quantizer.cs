using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace GameBot.Core.Quantizers
{
    public class Quantizer : CalibrateableQuantizer
    {
        public int ThresholdConstant { get; set; }
        public int ThresholdBlockSize { get; set; }

        private readonly int _thresholdMaxValue = 255;
        private readonly AdaptiveThresholdType _thresholdAdaptiveThresholdType = AdaptiveThresholdType.MeanC;
        private readonly ThresholdType _thresholdType = ThresholdType.Binary;
        private readonly bool _blurEnabled;

        public Quantizer(IConfig config)
        {
            var keypoints = config.ReadCollection("Robot.Quantizer.Transformation.KeyPoints", new[] { 0 + 100, 0, 640 - 100, 0, 0, 480, 640, 480 }).ToList();
            if (keypoints.Count != 8) throw new ArgumentException("Illegal value for config 'Robot.Quantizer.Transformation.KeyPoints'.");

            ThresholdConstant = config.Read("Robot.Quantizer.Threshold.Constant", 5);
            if (ThresholdConstant < 0) throw new ArgumentException("Illegal value for config 'Robot.Quantizer.Threshold.Constant'.");

            ThresholdBlockSize = config.Read("Robot.Quantizer.Threshold.BlockSize", 13);
            if (ThresholdBlockSize < 0 || ThresholdBlockSize % 2 == 0) throw new ArgumentException("Illegal value for config 'Robot.Quantizer.Threshold.BlockSize'.");
            
            _blurEnabled = config.Read("Robot.Quantizer.Blur", false);

            // precalculate transformation matrix
            Keypoints = new List<Point> { new Point(keypoints[0], keypoints[1]), new Point(keypoints[2], keypoints[3]), new Point(keypoints[4], keypoints[5]), new Point(keypoints[6], keypoints[7]) };
        }

        public override Mat Quantize(Mat image)
        {
            // convert to gray values
            Mat imageGray = new Mat();
            if (image.NumberOfChannels > 1)
            {
                CvInvoke.CvtColor(image, imageGray, ColorConversion.Rgb2Gray);
            }
            else
            {
                imageGray = image;
            }

            // transform
            var imageWarped = new Mat(new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight), DepthType.Default, 1);
            CvInvoke.WarpPerspective(imageGray, imageWarped, Transform, new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight));

            if (_blurEnabled)
            {
                // gauss
                CvInvoke.GaussianBlur(imageWarped, imageWarped, new Size(3, 3), 0.6, 0.6);
            }

            // threshold
            var imageBinarized = new Mat(new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight), DepthType.Default, 1);
            CvInvoke.AdaptiveThreshold(imageWarped, imageBinarized, _thresholdMaxValue, _thresholdAdaptiveThresholdType, _thresholdType, ThresholdBlockSize, ThresholdConstant);

            return imageBinarized;
        }
    }
}
