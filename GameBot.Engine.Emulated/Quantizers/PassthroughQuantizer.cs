using Emgu.CV;
using GameBot.Core;

namespace GameBot.Engine.Emulated.Quantizers
{
    public class PassthroughQuantizer : IQuantizer
    {
        public IImage Quantize(IImage image)
        {
            return image;
        }
    }
}
