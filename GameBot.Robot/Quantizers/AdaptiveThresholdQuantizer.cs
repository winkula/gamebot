using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using GameBot.Core;
using GameBot.Core.Data;
using System;
using System.Diagnostics;
using System.Drawing;

namespace GameBot.Robot.Quantizers
{
    public class AdaptiveThresholdQuantizer : IQuantizer
    {
        private bool adjust;
        private int c = 5;
        private int block = 13;
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

        public IScreenshot Quantize(IImage image, TimeSpan timestamp)
        {
            var capture = new Capture();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            string filename = "Images/tetris_1.jpg";
            //var sourceImageMat = new Mat(filename, LoadImageType.Grayscale);
            //var sourceImageMat = new Mat();
            Mat sourceImageMat = null;
            for (int i = 0; i < 100; i++)
            {
                sourceImageMat = capture.QueryFrame();
            }
            var destImageMat = new Mat(sourceImageMat.Size, DepthType.Default, 1);
            var destImageBinMat = new Mat(sourceImageMat.Size, DepthType.Default, 1);

            Debug.WriteLine("Load: " + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();

            // calculate transformation matrix
            Matrix<float> sourceMat = new Matrix<float>(keypoints);
            Matrix<float> destMat = new Matrix<float>(new float[,] { { 0, 0 }, { 160, 0 }, { 0, 144 }, { 160, 144 } });
            var transform = CvInvoke.GetPerspectiveTransform(sourceMat, destMat);

            Debug.WriteLine("Transform: " + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();

            // transform
            CvInvoke.WarpPerspective(sourceImageMat, destImageMat, transform, new Size(160, 144), Inter.Linear, Warp.Default);

            Debug.WriteLine("Warp: " + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();

            // threshold
            //CvInvoke.AdaptiveThreshold(destImageMat, destImageBinMat, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, block, c);

            Debug.WriteLine("AdaptiveThreshold: " + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();

            //Image<Gray, Byte> sourceImage = new Image<Gray, Byte>(new Bitmap(image));
            //Image<Gray, Byte> destImage = new Image<Gray, Byte>(160, 144);
            //Image<Gray, Byte> destImageBin = new Image<Gray, Byte>(160, 144);


            //CvInvoke.WarpPerspective(sourceImage, destImage, transform, new Size(160, 144), Inter.Linear, Warp.Default);

            //CvInvoke.AdaptiveThreshold(destImage, destImageBin, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, block, c);

            while (adjust)
            {
                //CvInvoke.AdaptiveThreshold(destImage, destImageBin, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, block, c);
                //CvInvoke.AdaptiveThreshold(destImageMat, destImageBinMat, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, block, c);

                CvInvoke.NamedWindow("Test");
                CvInvoke.Imshow("Test", destImageMat);

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

            var screenshot = new Screenshot(destImageBinMat.Bitmap, new TimeSpan());
            return screenshot;
        }
    }
}
