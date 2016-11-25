using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace GameBot.Core.Quantizers
{
    public class SimpleThresholdQuantizer : CalibrateableQuantizer
    {
        private readonly ThresholdType _thresholdType = ThresholdType.Binary;

        public double Threshold { get; set; }

        public SimpleThresholdQuantizer()
        {
            Threshold = 255.0 / 2;
        }

        public override Mat Quantize(Mat image)
        {
            // convert to gray values
            Mat imageGray = new Mat();
            if (image.NumberOfChannels > 1)
            {
                CvInvoke.CvtColor(image, imageGray, ColorConversion.Rgb2Gray);
            }
            else
            {
                imageGray = image;
            }

            // transform
            var imageWarped = new Mat();
            CvInvoke.WarpPerspective(imageGray, imageWarped, Transform, new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight));
            
            // threshold
            var imageBinarized = new Mat();
            CvInvoke.Threshold(imageWarped, imageBinarized, Threshold, 255, _thresholdType);

            return imageBinarized;
        }
    }
}
