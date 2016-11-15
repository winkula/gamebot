using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace GameBot.Core.Quantizers
{
    public class MorphologyQuantizer : Quantizer
    {
        private readonly Mat _kernel;

        public MorphologyQuantizer(IConfig config) : base(config)
        {
            _kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(7, 7), new Point(-1, -1));
        }

        public override Mat Quantize(Mat image)
        {
            var dst = base.Quantize(image);

            CvInvoke.MorphologyEx(dst, dst, MorphOp.Open, _kernel, new Point(-1, -1), 1, BorderType.Constant, new MCvScalar(0));

            return dst;
        }
    }
}
