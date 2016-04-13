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
    }
}
