using Emgu.CV;
using GameBot.Core.Data;

namespace GameBot.Core
{
    /// <summary>
    /// Represents an agent who is able to play a Game Boy game based on screenshots of the device's display.
    /// The Agent is responsible to check, whether a command was executed by the engine.
    /// In ever loop the methods Extract, Visualize (optional) and Play are called.
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Extracts the game state.
        /// This is only needed because the Engine wants to separate Extraction and Play.
        /// Else we could have a single method instead of 'Extract' and 'Play'.
        /// </summary>
        /// <param name="screenshot">The current screenshot of the device's display.</param>
        void Extract(IScreenshot screenshot);

        /// <summary>
        /// Visualizes the current game state or the extracted information on the quantized image.
        /// This method is for the UI only.
        /// </summary>
        /// <param name="image">The quantized image.</param>
        /// <returns>A visualization on the quantized image.</returns>
        Mat Visualize(Mat image);

        /// <summary>
        /// Acts based on the current screenshot.
        /// </summary>
        /// <param name="executor">The executor to press buttons on the game boy.</param>
        void Play(IExecutor executor);
        
        /// <summary>
        /// Resets the internal state of the agent.
        /// </summary>
        void Reset();
    }
}
