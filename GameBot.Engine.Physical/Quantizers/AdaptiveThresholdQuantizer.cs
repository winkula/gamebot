using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using NLog;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace GameBot.Engine.Physical.Quantizers
{
    public class AdaptiveThresholdQuantizer : IQuantizer
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly bool _adjust;
        private int _c = 5;
        private int _block = 13;
        private AdaptiveThresholdType _mode = AdaptiveThresholdType.MeanC; 
        private readonly float[,] _keypoints = new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } };

        public IEnumerable<Point> Keypoints { get; set; }

        public AdaptiveThresholdQuantizer()
        {
        }

        public AdaptiveThresholdQuantizer(bool adjust, float[,] keypoints, int c, int block)
        {
            _adjust = adjust;
            _keypoints = keypoints;
            _c = c;
            _block = block;
        }

        public Mat Quantize(Mat image)
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

            /* Too resource-intensive!
            // denoise
            CvInvoke.FastNlMeansDenoising(sourceImage, sourceImage);

            _logger.Info($"{stopwatch.ElapsedMilliseconds} ms, FastNlMeansDenoising");
            stopwatch.Restart();
            */

            // transform
            CvInvoke.WarpPerspective(sourceImage, destImage, transform, new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight), Inter.Linear, Warp.Default);

            _logger.Info($"{stopwatch.ElapsedMilliseconds} ms, WarpPerspective");
            stopwatch.Restart();

            // threshold
            CvInvoke.AdaptiveThreshold(destImage, destImageBin, 255, _mode, ThresholdType.Binary, _block, _c);

            _logger.Info($"{stopwatch.ElapsedMilliseconds} ms, AdaptiveThreshold");
            stopwatch.Restart();

            while (_adjust)
            {
                CvInvoke.AdaptiveThreshold(destImage, destImageBin, 255, _mode, ThresholdType.Binary, _block, _c);

                CvInvoke.NamedWindow("Test");
                CvInvoke.Imshow("Test", destImageBin);


                string outputFilename = "quantizer_output.png";
                string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), outputFilename);
                destImageBin.Save(outputPath);

                int key = CvInvoke.WaitKey();
                Debug.Write(key);
                if (key == 2490368) _block += 2;
                if (key == 2621440) _block -= 2;
                if (key == 2424832) _c++;
                if (key == 2555904) _c--;
                if (key == 109) _mode = AdaptiveThresholdType.MeanC;
                if (key == 103) _mode = AdaptiveThresholdType.GaussianC;
                if (key == 27) break;

                if (_block < 3) _block = 3;

                _logger.Info("Constant: " + _c);
                _logger.Info("Block size: " + _block);
                _logger.Info("Mode: " + _mode);
            }
            
            return destImageBin;
        }
    }
}
