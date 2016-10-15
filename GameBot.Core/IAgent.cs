using Emgu.CV;
using GameBot.Core.Data;

namespace GameBot.Core
{
    /// <summary>
    /// Represents an agent who is able to play a Game Boy game based on screenshots of the device's display.
    /// The Agent is responsible to check, whether a command was executed by the engine.
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Acts based on the current screenshot.
        /// </summary>
        /// <param name="screenshot">The current screenshot of the device's display.</param>
        /// <param name="executor">The executor to press buttons on the game boy.</param>
        void Act(IScreenshot screenshot, IExecutor executor);
        
        /// <summary>
        /// Visualizes the current game state or the extracted information on the quantized image.
        /// This method is for the UI.
        /// </summary>
        /// <param name="image">The quantized image.</param>
        /// <returns>A visualization on the quantized image.</returns>
        IImage Visualize(IImage image);
    }
}
