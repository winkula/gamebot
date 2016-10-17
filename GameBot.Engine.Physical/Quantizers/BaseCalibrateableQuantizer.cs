using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using GameBot.Core;

namespace GameBot.Engine.Physical.Quantizers
{
    public abstract class BaseCalibrateableQuantizer : ICalibrateableQuantizer
    {
        protected Mat Transform { get; private set; }

        public abstract IImage Quantize(IImage image);

        public void Calibrate(IEnumerable<Point> keypoints)
        {
            if (keypoints == null) throw new ArgumentNullException(nameof(keypoints));
            var keypointList = keypoints.ToList();
            if (keypointList.Count != 4) throw new ArgumentException("keypoints must be four points");

            var srcKeypoints = new Matrix<float>(new float[,] { { keypointList[0].X, keypointList[0].Y }, { keypointList[1].X, keypointList[1].Y }, { keypointList[2].X, keypointList[2].Y }, { keypointList[3].X, keypointList[3].Y } });
            var destKeypoints = new Matrix<float>(new float[,] { { 0, 0 }, { GameBoyConstants.ScreenWidth, 0 }, { 0, GameBoyConstants.ScreenHeight }, { GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight } });
            Transform = CvInvoke.GetPerspectiveTransform(srcKeypoints, destKeypoints);
        }
    }
}
