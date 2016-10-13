using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using NLog;
using System.Diagnostics;
using System.Drawing;

namespace GameBot.Engine.Physical.Quantizers
{
    public class AdaptiveThresholdQuantizer : IQuantizer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private bool adjust;
        private int c = 5;
        private int block = 13;
        private AdaptiveThresholdType mode = AdaptiveThresholdType.MeanC; 
        private float[,] keypoints = new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } };

        public AdaptiveThresholdQuantizer()
        {
        }

        public AdaptiveThresholdQuantizer(bool adjust, float[,] keypoints, int c, int block)
        {
            this.adjust = adjust;
            this.keypoints = keypoints;
            this.c = c;
            this.block = block;
        }

        public IImage Quantize(IImage image)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var sourceImage = image;
            var destImage = new Mat(sourceImage.Size, DepthType.Default, 1);
            var destImageBin = new Mat(sourceImage.Size, DepthType.Default, 1);            
            Matrix<float> srcKeypoints = new Matrix<float>(keypoints);
            Matrix<float> destKeypoints = new Matrix<float>(new float[,] { { 0, 0 }, { GameBoyConstants.ScreenWidth, 0 }, { 0, GameBoyConstants.ScreenHeight }, { GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight } });

            // calculate transformation matrix
            var transform = CvInvoke.GetPerspectiveTransform(srcKeypoints, destKeypoints);

            logger.Info($"{stopwatch.ElapsedMilliseconds} ms, GetPerspectiveTransform");
            stopwatch.Restart();

            // transform
            CvInvoke.WarpPerspective(sourceImage, destImage, transform, new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight), Inter.Linear, Warp.Default);

            logger.Info($"{stopwatch.ElapsedMilliseconds} ms, WarpPerspective");
            stopwatch.Restart();

            // threshold
            CvInvoke.AdaptiveThreshold(destImage, destImageBin, 255, mode, ThresholdType.Binary, block, c);

            logger.Info($"{stopwatch.ElapsedMilliseconds} ms, AdaptiveThreshold");
            stopwatch.Restart();

            while (adjust)
            {
                CvInvoke.AdaptiveThreshold(destImage, destImageBin, 255, mode, ThresholdType.Binary, block, c);

                CvInvoke.NamedWindow("Test");
                CvInvoke.Imshow("Test", destImageBin);

                int key = CvInvoke.WaitKey();
                Debug.Write(key);
                if (key == 2490368) block += 2;
                if (key == 2621440) block -= 2;
                if (key == 2424832) c++;
                if (key == 2555904) c--;
                if (key == 109) mode = AdaptiveThresholdType.MeanC;
                if (key == 103) mode = AdaptiveThresholdType.GaussianC;
                if (key == 27) break;

                if (block < 3) block = 3;

                logger.Info("Constant: " + c);
                logger.Info("Block size: " + block);
                logger.Info("Mode: " + mode);
            }
            
            return destImageBin;
        }
    }
}
