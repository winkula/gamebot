using GameBot.Core.Data;
using System.Collections.Generic;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a solver for a Game Boy game.
    /// </summary>
    public interface ISolver
    {
    }

    /// <summary>
    /// Represents a solver for a specific Game Boy game.
    /// </summary>
    /// <typeparam name="T">Type of the game state.</typeparam>
    public interface ISolver<T> : ISolver where T : class, IGameState
    {
        /// <summary>
        /// Solves a Game Boy game, i. e. decides, which commands to press on the device for a given game state.
        /// </summary>
        /// <param name="gameState">The game state.</param>
        /// <returns>A collection of commands to execute on the device.</returns>
        IEnumerable<ICommand> Solve(T gameState);
    }
}
