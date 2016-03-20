using System;

namespace GameBot.Core.Data
{
    /// <summary>
    /// Represents the display state of the Game Boy device. In other words a screenshot.
    /// The origin of the coordinate system is top left.
    /// </summary>
    public interface IScreenshot
    {
        /// <summary>
        /// Gets the pixels of the screenshot row by row.
        /// </summary>
        int[] Pixels { get; }

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
        int GetPixel(int x, int y);

        /// <summary>
        /// Gets a tile, i.e. an 8 by 8 pixel sqare, of the screenshot.
        /// </summary>
        /// <param name="x">The x coordinate of the tile (from left).</param>
        /// <param name="y">The y coordinate of the tile (from top).</param>
        /// <returns>An array, representing the pixels of the tile row by row.</returns>
        int[] GetTile(int x, int y);
    }
}
