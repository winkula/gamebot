using System;

namespace GameBot.Core.Data
{
    /// <summary>
    /// Represents a game state of a Game Boy game.
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Gets the moment in time, when this game state was relevant.
        /// </summary>
        TimeSpan Timestamp { get; }
    }
}
