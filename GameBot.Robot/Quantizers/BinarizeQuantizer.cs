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
    public class BinarizeQuantizer : IQuantizer
    {
        private bool adjust;
        private int threshold = 50;
        private float[,] keypoints = new float[,] { { 488, 334 }, { 1030, 333 }, { 435, 813 }, { 1061, 811 } };

        public BinarizeQuantizer()
        {
        }

        public BinarizeQuantizer(bool adjust, float[,] keypoints, int threshold)
        {
            this.adjust = adjust;
            this.keypoints = keypoints;
            this.threshold = threshold;
        }

        public IScreenshot Quantize(Image image, TimeSpan timestamp)
        {
            Image<Gray, Byte> sourceImage = new Image<Gray, Byte>(new Bitmap(image));
            Image<Gray, Byte> destImage = new Image<Gray, Byte>(160, 144);
            Image<Gray, Byte> destImageBin = new Image<Gray, Byte>(160, 144);

            Matrix<float> sourceMat = new Matrix<float>(keypoints);
            Matrix<float> destMat = new Matrix<float>(new float[,] { { 0, 0 }, { 160, 0 }, { 0, 144 }, { 160, 144 } });

            var transform = CvInvoke.GetPerspectiveTransform(sourceMat, destMat);
            CvInvoke.WarpPerspective(sourceImage, destImage, transform, new Size(160, 144), Inter.Linear, Warp.Default);
            
            CvInvoke.Threshold(destImage, destImageBin, threshold, 255, ThresholdType.Binary);

            while (adjust)
            {
                CvInvoke.Threshold(destImage, destImageBin, threshold, 255, ThresholdType.Binary);

                CvInvoke.NamedWindow("Test");
                CvInvoke.Imshow("Test", destImageBin);

                int key = CvInvoke.WaitKey();
                if (key == 2424832) threshold++;
                if (key == 2555904) threshold--;
                if (key == 27) break;

                Debug.WriteLine("Threshold: " + threshold);
            }

            var screenshot = new Screenshot(destImageBin.ToBitmap(), new TimeSpan());
            return screenshot;
        }
    }
}
