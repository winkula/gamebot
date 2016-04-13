using Emgu.CV;
using GameBot.Core.Data;
using System;

namespace GameBot.Core
{
    /// <summary>
    /// Represents an image quantizer to get a quantized screenshot from a noisy image of a Game Boy display.
    /// </summary>
    public interface IQuantizer
    {
        /// <summary>
        /// Quantizes a noisy image or photo of the device's display.
        /// </summary>
        /// <param name="image">Noisy image or photos of the device's display.</param>
        /// <param name="timestamp">The current timestamp.</param>
        /// <returns>A quantized screenshot of the device's display.</returns>
        IScreenshot Quantize(IImage image, TimeSpan timestamp);
    }
}
