using GameBot.Core;
using GameBot.Core.Data;
using System;
using System.Drawing;

namespace GameBot.Robot.Quantizers
{
    public class PassthroughQuantizer : IQuantizer
    {
        public IScreenshot Quantize(Image image, TimeSpan timestamp)
        {
            return new Screenshot(image, timestamp);
        }
    }
}
