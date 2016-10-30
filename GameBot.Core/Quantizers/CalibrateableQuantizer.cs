using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;

namespace GameBot.Core.Quantizers
{
    public abstract class CalibrateableQuantizer : IQuantizer
    {
        protected Mat Transform { get; private set; }

        private IList<Point> _keypoints;
        public IEnumerable<Point> Keypoints
        {
            get
            {
                return _keypoints;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                var keypointList = value.ToList();
                if (keypointList.Count != 4) throw new ArgumentException("keypoints must be four points");

                _keypoints = keypointList;

                var srcKeypoints = new Matrix<float>(new float[,] {
                    {keypointList[0].X, keypointList[0].Y},
                    { keypointList[1].X, keypointList[1].Y },
                    { keypointList[2].X, keypointList[2].Y },
                    { keypointList[3].X, keypointList[3].Y }
                });
                var destKeypoints = new Matrix<float>(new float[,]
                {
                    { 0, 0 },
                    { GameBoyConstants.ScreenWidth, 0 },
                    { 0, GameBoyConstants.ScreenHeight },
                    { GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight }
                });

                Transform = CvInvoke.GetPerspectiveTransform(srcKeypoints, destKeypoints);
            }
        }

        public abstract IImage Quantize(IImage image);
    }
}
