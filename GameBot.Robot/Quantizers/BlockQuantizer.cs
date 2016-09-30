using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using GameBot.Core;
using GameBot.Core.Data;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace GameBot.Robot.Quantizers
{
    public class BlockQuantizer : IQuantizer
    {
        private bool adjust;
        private int c = 5;
        private int block = 13;
        private int threshold = 195;
        private float[,] keypoints = new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } };

        public BlockQuantizer()
        {
        }

        public BlockQuantizer(bool adjust, float[,] keypoints, int c, int block)
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
            Matrix<float> destKeypoints = new Matrix<float>(new float[,] { { 0, 0 }, { 160, 0 }, { 0, 144 }, { 160, 144 } });

            // calculate transformation matrix
            var transform = CvInvoke.GetPerspectiveTransform(srcKeypoints, destKeypoints);

            Debug.WriteLine($"{stopwatch.ElapsedMilliseconds} ms, GetPerspectiveTransform");
            stopwatch.Restart();

            // transform
            CvInvoke.WarpPerspective(sourceImage, destImage, transform, new Size(160, 144), Inter.Linear, Warp.Default);

            Debug.WriteLine($"{stopwatch.ElapsedMilliseconds} ms, WarpPerspective");
            stopwatch.Restart();

            // threshold
            CvInvoke.AdaptiveThreshold(destImage, destImageBin, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, block, c);

            Debug.WriteLine($"{stopwatch.ElapsedMilliseconds} ms, AdaptiveThreshold");
            stopwatch.Restart();

            // get blocks
            var black = new Mat(new Size(160, 144), DepthType.Cv8U, 1);
            black.SetTo(new MCvScalar(0, 0, 0));
            var sb = new StringBuilder();
            for (int y = 0; y < 144 / 8; y++)
            {
                for (int x = 0; x < 160 / 8; x++)
                {
                    var mask = black.Clone();
                    var roi = new Rectangle(x * 8, y * 8, 8, 8);
                    CvInvoke.Rectangle(mask, roi, new MCvScalar(255, 255, 255), -1);

                    //CvInvoke.Imshow("Mask", mask);
                    //CvInvoke.WaitKey();

                    var mean = CvInvoke.Mean(destImageBin, mask);
                    bool isBlack = mean.V0 < 190; // Optimum: zwischen. 185 und 195;
                    sb.Append(isBlack ? "#" : ".");
                }
                sb.AppendLine();
            }
            Debug.Write(sb.ToString());

            Debug.WriteLine($"{stopwatch.ElapsedMilliseconds} ms, GetBlocks");
            stopwatch.Restart();

            CvInvoke.Imshow("Test", destImageBin);

            while (adjust)
            {
                CvInvoke.AdaptiveThreshold(destImage, destImageBin, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, block, c);

                CvInvoke.NamedWindow("Test");
                CvInvoke.Imshow("Test", destImageBin);

                int key = CvInvoke.WaitKey();
                if (key == 2490368) block += 2;
                if (key == 2621440) block -= 2;
                if (key == 2424832) c++;
                if (key == 2555904) c--;
                if (key == 27) break;

                if (block < 3) block = 3;

                Debug.WriteLine("Constant: " + c);
                Debug.WriteLine("Block size: " + block);
            }

            return destImageBin;
        }
    }
}
