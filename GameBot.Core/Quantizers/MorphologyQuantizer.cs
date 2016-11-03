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
            // TODO: use a diagonal cross instad of a square?
            _kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(7, 7), new Point(-1, -1));
        }

        public override IImage Quantize(IImage image)
        {
            var dst = base.Quantize(image);
            CvInvoke.MorphologyEx(dst, dst, MorphOp.Open, _kernel, new Point(-1, -1), 1, BorderType.Replicate, new MCvScalar(-1));

            return dst;
        }
    }
}
