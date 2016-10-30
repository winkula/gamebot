using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using NLog;
using System.Diagnostics;
using System.Drawing;

namespace GameBot.Engine.Physical.Quantizers
{
    public class ThresholdQuantizer : IQuantizer
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly bool _adjust;
        private int _threshold = 50;
        private readonly float[,] _keypoints = new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } };

        public IEnumerable<Point> Keypoints { get; set; }

        public ThresholdQuantizer()
        {
        }

        public ThresholdQuantizer(bool adjust, float[,] keypoints, int threshold)
        {
            _adjust = adjust;
            _keypoints = keypoints;
            _threshold = threshold;
        }

        public IImage Quantize(IImage image)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var sourceImage = image;
            var destImage = new Mat(sourceImage.Size, DepthType.Default, 1);
            var destImageBin = new Mat(sourceImage.Size, DepthType.Default, 1);
            Matrix<float> srcKeypoints = new Matrix<float>(_keypoints);
            Matrix<float> destKeypoints = new Matrix<float>(new float[,] { { 0, 0 }, { GameBoyConstants.ScreenWidth, 0 }, { 0, GameBoyConstants.ScreenHeight }, { GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight } });
            
            // calculate transformation matrix
            var transform = CvInvoke.GetPerspectiveTransform(srcKeypoints, destKeypoints);

            _logger.Info($"{stopwatch.ElapsedMilliseconds} ms, GetPerspectiveTransform");
            stopwatch.Restart();

            // transform
            CvInvoke.WarpPerspective(sourceImage, destImage, transform, new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight), Inter.Linear, Warp.Default);

            _logger.Info($"{stopwatch.ElapsedMilliseconds} ms, WarpPerspective");
            stopwatch.Restart();

            // threshold
            CvInvoke.Threshold(destImage, destImageBin, _threshold, 255, ThresholdType.Binary);

            _logger.Info($"{stopwatch.ElapsedMilliseconds} ms, Threshold");
            stopwatch.Restart();

            while (_adjust)
            {
                CvInvoke.Threshold(destImage, destImageBin, _threshold, 255, ThresholdType.Binary);

                CvInvoke.NamedWindow("Test");
                CvInvoke.Imshow("Test", destImageBin);

                int key = CvInvoke.WaitKey();
                if (key == 2424832) _threshold++;
                if (key == 2555904) _threshold--;
                if (key == 27) break;

                _logger.Info("Threshold: " + _threshold);
            }

            return destImageBin;
        }
    }
}
