using Emgu.CV;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a regular camera who captures images.
    /// </summary>
    public interface ICamera
    {
        /// <summary>
        /// Returns the current image from the camera.
        /// </summary>
        /// <returns>The current image from the camera.</returns>
        Mat Capture();

        /// <summary>
        /// Ensures that a new image is returned from the camera.
        /// </summary>
        /// <param name="predecessor">The last captures image.</param>
        /// <returns>The current image from the camera.</returns>
        Mat Capture(Mat predecessor);

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
