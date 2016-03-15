using System.Drawing;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a regular camera who captures images.
    /// </summary>
    public interface ICamera
    {
        /// <summary>
        /// Captures an image.
        /// </summary>
        /// <returns>The captured image from the camera.</returns>
        Image Capture();
    }
}
