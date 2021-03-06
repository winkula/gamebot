﻿using System;
using Emgu.CV;

namespace GameBot.Core.Data
{
    /// <summary>
    /// Represents the display state of the Game Boy device. In other words a screenshot.
    /// The origin of the coordinate system is top left.
    /// Pixel values go from 0 (black) to 255 (white).
    /// </summary>
    public interface IScreenshot
    {
        /// <summary>
        /// Gets or sets the original underlying EmguCV image.
        /// </summary>
        Mat OriginalImage { get; set; }

        /// <summary>
        /// Gets the processed underlying EmguCV image.
        /// </summary>
        Mat Image { get; }

        /// <summary>
        /// Gets the width of the screenshot.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the screenshot.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the moment in time, when the screenshot was captured.
        /// </summary>
        TimeSpan Timestamp { get; }

        /// <summary>
        /// Gets one specific pixel of the screenshot.
        /// </summary>
        byte GetPixel(int x, int y);
        
        /// <summary>
        /// Gets the mean value of a tile (8 by 8 pixel sqare) of the screenshot.
        /// </summary>
        /// <param name="x">The x coordinate of the tile (from left).</param>
        /// <param name="y">The y coordinate of the tile (from top).</param>
        /// <returns>The mean value of the tile.</returns>
        byte GetTileMean(int x, int y);
    }
}
