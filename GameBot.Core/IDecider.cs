using GameBot.Core.Data;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a decider for a Game Boy game.
    /// </summary>
    public interface IDecider
    {
    }

    /// <summary>
    /// Represents a decider for a specific Game Boy game.
    /// </summary>
    /// <typeparam name="T">Type of the game state.</typeparam>
    public interface IDecider<T> : IDecider where T : IGameState, new()
    {
        /// <summary>
        /// Decides, which commands to press on the device for a given game state.
        /// </summary>
        /// <param name="gameState">The game state.</param>
        /// <param name="context">The context of the game state.</param>
        /// <returns>A collection of commands to execute on the device.</returns>
        ICommands Decide(T gameState, IContext<T> context);
    }
}
