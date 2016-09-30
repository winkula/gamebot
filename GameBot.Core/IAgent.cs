using Emgu.CV;
using GameBot.Core.Data;
using System.Collections.Generic;

namespace GameBot.Core
{
    /// <summary>
    /// Represents an agent who is able to play a Game Boy game based on screenshots of the device's display.
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Acts based on the current screenshot.
        /// </summary>
        /// <param name="screenshot">The current screenshot of the device's display.</param>
        /// <returns>A collection of commands for the device.</returns>
        IEnumerable<ICommand> Act(IScreenshot screenshot);

        /// <summary>
        /// Checks, if the last command was executed correctly.
        /// </summary>
        /// <param name="command">The command to check.</param>
        /// <returns><code>True</code> if the command was executed correctly.</returns>
        bool Check(ICommand command);

        /// <summary>
        /// Visualizes the current game state or the extracted information on the quantized image.
        /// This method is for the UI.
        /// </summary>
        /// <param name="image">The quantized image.</param>
        /// <returns>A visualization on the quantized image.</returns>
        IImage Visualize(IImage image);
    }
}
