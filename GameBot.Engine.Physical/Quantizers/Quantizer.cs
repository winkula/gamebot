using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameBot.Engine.Physical.Quantizers
{
    public class Quantizer : IQuantizer
    {
        private const int GameBoyScreenWidth = GameBoyConstants.ScreenWidth;
        private const int GameBoyScreenHeight = GameBoyConstants.ScreenHeight;

        private readonly IConfig config;

        private int thresholdConstant;
        private int thresholdBlockSize;
        private int thresholdMaxValue;
        private AdaptiveThresholdType thresholdAdaptiveThresholdType;
        private ThresholdType thresholdType;

        public Mat transform { get; private set; }

        public Quantizer(IConfig config)
        {
            this.config = config;

            var keypoints = config.ReadCollection("Robot.Quantizer.Transformation.KeyPoints", new int[] { 0 + 100, 0, 640 - 100, 0, 0, 480, 640, 480 }).ToList();
            if (keypoints.Count != 8) throw new ArgumentException("Illegal value for config 'Robot.Quantizer.Transformation.KeyPoints'.");

            thresholdConstant = config.Read("Robot.Quantizer.Threshold.Constant", 5);
            if (thresholdConstant < 0) throw new ArgumentException("Illegal value for config 'Robot.Quantizer.Threshold.Constant'.");

            thresholdBlockSize = config.Read("Robot.Quantizer.Threshold.BlockSize", 13);
            if (thresholdBlockSize < 0 || thresholdBlockSize % 2 == 0) throw new ArgumentException("Illegal value for config 'Robot.Quantizer.Threshold.BlockSize'.");

            thresholdMaxValue = config.Read("Robot.Quantizer.Threshold.MaxValue", 13);
            if (thresholdMaxValue < 0 || thresholdMaxValue > 255) throw new ArgumentException("Illegal value for config 'Robot.Quantizer.Threshold.MaxValue'.");

            thresholdAdaptiveThresholdType = config.Read("Robot.Quantizer.Threshold.AdaptiveThresholdType", AdaptiveThresholdType.MeanC);

            thresholdType = config.Read("Robot.Quantizer.Threshold.ThresholdType", ThresholdType.Binary);

            // precalculate transformation matrix
            Calibrate(new List<Point> { new Point(keypoints[0], keypoints[1]), new Point(keypoints[2], keypoints[3]), new Point(keypoints[4], keypoints[5]), new Point(keypoints[6], keypoints[7]) });
        }

        public void Calibrate(IEnumerable<Point> keypoints)
        {
            if (keypoints == null) throw new ArgumentNullException(nameof(keypoints));
            if (keypoints.Count() != 4) throw new ArgumentException("keypoints must be four points");

            var keypointsArray = keypoints.ToArray();
            var srcKeypoints = new Matrix<float>(new float[,] { { keypointsArray[0].X, keypointsArray[0].Y }, { keypointsArray[1].X, keypointsArray[1].Y }, { keypointsArray[2].X, keypointsArray[2].Y }, { keypointsArray[3].X, keypointsArray[3].Y } });
            var destKeypoints = new Matrix<float>(new float[,] { { 0, 0 }, { GameBoyScreenWidth, 0 }, { 0, GameBoyScreenHeight }, { GameBoyScreenWidth, GameBoyScreenHeight } });
            transform = CvInvoke.GetPerspectiveTransform(srcKeypoints, destKeypoints);
        }

        public IImage Quantize(IImage image)
        {
            // convert to gray values
            var imageGray = new Mat();
            CvInvoke.CvtColor(image, imageGray, ColorConversion.Rgb2Gray);

            // transform
            var imageWarped = new Mat(new Size(GameBoyScreenWidth, GameBoyScreenHeight), DepthType.Default, 1);
            CvInvoke.WarpPerspective(imageGray, imageWarped, transform, new Size(GameBoyScreenWidth, GameBoyScreenHeight), Inter.Linear, Warp.Default);

            // threshold
            var imageBinarized = new Mat(new Size(GameBoyScreenWidth, GameBoyScreenHeight), DepthType.Default, 1);
            CvInvoke.AdaptiveThreshold(imageWarped, imageBinarized, thresholdMaxValue, AdaptiveThresholdType.MeanC, ThresholdType.Binary, thresholdBlockSize, thresholdConstant);

            return imageBinarized;
        }
    }
}
