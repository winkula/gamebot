using System.Collections.Generic;
using System.Drawing;

namespace GameBot.Core
{
    /// <summary>
    /// Represents an image quantizer who can be calibrated with keypoints.
    /// </summary>
    public interface ICalibrateableQuantizer : IQuantizer
    {
        /// <summary>
        /// Gets the four keypoints of the the perspective transform.
        /// </summary>
        IEnumerable<Point> Keypoints { get; }

        /// <summary>
        /// Sets four keypoints to calculate the perspective transform of the quantizer.
        /// </summary>
        /// <param name="keypoints">A list of four keypoints</param>
        void Calibrate(IEnumerable<Point> keypoints);
    }
}
