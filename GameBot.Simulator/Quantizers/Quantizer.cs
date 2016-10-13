using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameBot.Robot.Quantizers
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

            var keypoints = config.ReadCollection("Robot.Quantizer.Transformation.KeyPoints", new float[] { 0 + 100, 0, 640 - 100, 0, 0, 480, 640, 480 }).ToList();
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
            CalculatePerspectiveTransform(keypoints);
        }

        public void CalculatePerspectiveTransform(IEnumerable<float> keypoints)
        {
            var keypointsArray = keypoints.ToArray();
            var srcKeypoints = new Matrix<float>(new float[,] { { keypointsArray[0], keypointsArray[1] }, { keypointsArray[2], keypointsArray[3] }, { keypointsArray[4], keypointsArray[5] }, { keypointsArray[6], keypointsArray[7] } });
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
