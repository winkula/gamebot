using Emgu.CV;
using GameBot.Core;

namespace GameBot.Robot.Quantizers
{
    public class PassthroughQuantizer : IQuantizer
    {
        public IImage Quantize(IImage image)
        {
            return image;
        }
    }
}
