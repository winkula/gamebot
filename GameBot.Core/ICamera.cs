using Emgu.CV;

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
        /// <returns>The captured image from the camera as an EmguCV Mat object.</returns>
        Mat Capture();
        
        /// <summary>
        /// Gets the width of the camera image.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the camera image.
        /// </summary>
        int Height { get; }
    }
}
