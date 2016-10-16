using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameBot.Engine.Physical.Quantizers
{
    public class Quantizer : ICalibrateableQuantizer
    {
        private const int _gameBoyScreenWidth = GameBoyConstants.ScreenWidth;
        private const int _gameBoyScreenHeight = GameBoyConstants.ScreenHeight;

        private readonly int _thresholdConstant;
        private readonly int _thresholdBlockSize;
        private readonly int _thresholdMaxValue;
        private readonly AdaptiveThresholdType _thresholdAdaptiveThresholdType;
        private readonly ThresholdType _thresholdType;

        private Mat Transform { get; set; }

        public Quantizer(IConfig config)
        {
            var keypoints = config.ReadCollection("Robot.Quantizer.Transformation.KeyPoints", new[] { 0 + 100, 0, 640 - 100, 0, 0, 480, 640, 480 }).ToList();
            if (keypoints.Count != 8) throw new ArgumentException("Illegal value for config 'Robot.Quantizer.Transformation.KeyPoints'.");

            _thresholdConstant = config.Read("Robot.Quantizer.Threshold.Constant", 5);
            if (_thresholdConstant < 0) throw new ArgumentException("Illegal value for config 'Robot.Quantizer.Threshold.Constant'.");

            _thresholdBlockSize = config.Read("Robot.Quantizer.Threshold.BlockSize", 13);
            if (_thresholdBlockSize < 0 || _thresholdBlockSize % 2 == 0) throw new ArgumentException("Illegal value for config 'Robot.Quantizer.Threshold.BlockSize'.");

            _thresholdMaxValue = config.Read("Robot.Quantizer.Threshold.MaxValue", 13);
            if (_thresholdMaxValue < 0 || _thresholdMaxValue > 255) throw new ArgumentException("Illegal value for config 'Robot.Quantizer.Threshold.MaxValue'.");

            _thresholdAdaptiveThresholdType = config.Read("Robot.Quantizer.Threshold.AdaptiveThresholdType", AdaptiveThresholdType.MeanC);

            _thresholdType = config.Read("Robot.Quantizer.Threshold.ThresholdType", ThresholdType.Binary);

            // precalculate transformation matrix
            Calibrate(new List<Point> { new Point(keypoints[0], keypoints[1]), new Point(keypoints[2], keypoints[3]), new Point(keypoints[4], keypoints[5]), new Point(keypoints[6], keypoints[7]) });
        }

        public void Calibrate(IEnumerable<Point> keypoints)
        {
            if (keypoints == null) throw new ArgumentNullException(nameof(keypoints));
            var keypointList = keypoints.ToList();
            if (keypointList.Count != 4) throw new ArgumentException("keypoints must be four points");

            var srcKeypoints = new Matrix<float>(new float[,] { { keypointList[0].X, keypointList[0].Y }, { keypointList[1].X, keypointList[1].Y }, { keypointList[2].X, keypointList[2].Y }, { keypointList[3].X, keypointList[3].Y } });
            var destKeypoints = new Matrix<float>(new float[,] { { 0, 0 }, { _gameBoyScreenWidth, 0 }, { 0, _gameBoyScreenHeight }, { _gameBoyScreenWidth, _gameBoyScreenHeight } });
            Transform = CvInvoke.GetPerspectiveTransform(srcKeypoints, destKeypoints);
        }

        public IImage Quantize(IImage image)
        {
            // convert to gray values
            var imageGray = new Mat();
            CvInvoke.CvtColor(image, imageGray, ColorConversion.Rgb2Gray);

            // transform
            var imageWarped = new Mat(new Size(_gameBoyScreenWidth, _gameBoyScreenHeight), DepthType.Default, 1);
            CvInvoke.WarpPerspective(imageGray, imageWarped, Transform, new Size(_gameBoyScreenWidth, _gameBoyScreenHeight));

            // gauss
            CvInvoke.GaussianBlur(imageWarped, imageWarped, new Size(3, 3), 0.6, 0.6);

            // threshold
            var imageBinarized = new Mat(new Size(_gameBoyScreenWidth, _gameBoyScreenHeight), DepthType.Default, 1);
            CvInvoke.AdaptiveThreshold(imageWarped, imageBinarized, _thresholdMaxValue, _thresholdAdaptiveThresholdType, _thresholdType, _thresholdBlockSize, _thresholdConstant);

            return imageBinarized;
        }
    }
}
