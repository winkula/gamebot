using GameBot.Core.Data;
using System.Collections.Generic;

namespace GameBot.Core
{
    /// <summary>
    /// Represents the context of a Game Boy game state.
    /// </summary>
    public interface IContext
    {
    }

    /// <summary>
    /// Represents the context of a specific Game Boy game state.
    /// </summary>
    /// <typeparam name="T">Type of the game state.</typeparam>
    public interface IContext<T> : IContext where T : IGameState, new()
    {
        /// <summary>
        /// Gets the last few extracted game states.
        /// </summary>
        ICollection<T> GameStates { get; }

        /// <summary>
        /// Adds the latest extracted game state to the collection.
        /// </summary>
        /// <param name="gameState">The latest extracted game state.</param>
        void Add(T gameState);
    }
}
