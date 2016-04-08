using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Robot.Data;
using System;

namespace GameBot.Robot.Quantizers
{
    public class PassthroughQuantizer : IQuantizer
    {
        public IScreenshot Quantize(IImage image, TimeSpan timestamp)
        {
            return new EmguScreenshot(image, timestamp);
        }
    }
}
