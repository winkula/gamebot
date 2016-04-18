using GameBot.Core.Data;
using System.Collections.Generic;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a player or an artificial intelligence for a Game Boy game.
    /// </summary>
    public interface IPlayer
    {
    }

    /// <summary>
    /// Represents a player or an artificial intelligence for a specific Game Boy game.
    /// </summary>
    /// <typeparam name="T">Type of the game state.</typeparam>
    public interface IPlayer<T> : IPlayer where T : class, IGameState
    {
        /// <summary>
        /// Plays a Game Boy game, i. e. decides, which commands to press on the device for a given game state.
        /// </summary>
        /// <param name="gameState">The game state.</param>
        /// <returns>A collection of commands to execute on the device.</returns>
        IEnumerable<ICommand> Play(T gameState);
    }
}
