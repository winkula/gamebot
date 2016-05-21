using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using GameBot.Core.Data;
using System;
using System.Drawing;
using System.Linq;

namespace GameBot.Robot.Quantizers
{
    public class Quantizer : IQuantizer
    {
        private const int GameBoyScreenWidth = 160;
        private const int GameBoyScreenHeight = 144;

        private readonly IConfig config;

        private int thresholdConstant;
        private int thresholdBlockSize;
        private int thresholdMaxValue;
        private AdaptiveThresholdType thresholdAdaptiveThresholdType;
        private ThresholdType thresholdType;

        private readonly Mat transform;

        // for debug purposes only
        public IImage ImageOutput;

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
            var srcKeypoints = new Matrix<float>(new float[,] { { keypoints[0], keypoints[1] }, { keypoints[2], keypoints[3] }, { keypoints[4], keypoints[5] }, { keypoints[6], keypoints[7] } });
            var destKeypoints = new Matrix<float>(new float[,] { { 0, 0 }, { GameBoyScreenWidth, 0 }, { 0, GameBoyScreenHeight }, { GameBoyScreenWidth, GameBoyScreenHeight } });
            transform = CvInvoke.GetPerspectiveTransform(srcKeypoints, destKeypoints);
        }

        public IScreenshot Quantize(IImage image, TimeSpan timestamp)
        {
            var sourceImage = image;
            var destImage = new Mat(new Size(GameBoyScreenWidth, GameBoyScreenHeight), DepthType.Default, 1);
            var destImageBin = new Mat(new Size(GameBoyScreenWidth, GameBoyScreenHeight), DepthType.Default, 1);

            // transform
            CvInvoke.WarpPerspective(sourceImage, destImage, transform, new Size(GameBoyScreenWidth, GameBoyScreenHeight), Inter.Linear, Warp.Default);

            // threshold
            CvInvoke.AdaptiveThreshold(destImage, destImageBin, thresholdMaxValue, AdaptiveThresholdType.MeanC, ThresholdType.Binary, thresholdBlockSize, thresholdConstant);

            if (config.Read<bool>("Robot.Quantizer.Imshow", true))
            {
                CvInvoke.Imshow("Image_Binarized", destImageBin);
            }
            ImageOutput = destImageBin;

            return new EmguScreenshot(destImageBin, timestamp);
        }
    }
}
