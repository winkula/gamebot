﻿using Emgu.CV;

namespace GameBot.Core
{
    /// <summary>
    /// Represents an image quantizer to get a quantized image from a noisy image of a Game Boy display.
    /// </summary>
    public interface IQuantizer
    {
        /// <summary>
        /// Quantizes a noisy image or photo of the device's display.
        /// </summary>
        /// <param name="image">Noisy image or photos of the device's display.</param>
        /// <returns>A quantized image of the device's display.</returns>
        IImage Quantize(IImage image);
    }
}
