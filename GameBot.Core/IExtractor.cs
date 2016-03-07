using GameBot.Core.Data;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a extractor for game states from a screenshot of a Game Boy display.
    /// </summary>
    public interface IExtractor
    {
    }

    /// <summary>
    /// Represents a extractor for a specific type of game states from a screenshot of a Game Boy display.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IExtractor<T> : IExtractor where T : class, IGameState
    {
        /// <summary>
        /// Extracts the game state from a screenshot.
        /// </summary>
        /// <param name="screenshot">Screenshot of the device's display.</param>
        /// <param name="context">The context of the game state.</param>
        /// <returns>The extracted game state.</returns>
        T Extract(IScreenshot screenshot, IContext<T> context);
    }
}
