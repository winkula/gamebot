using GameBot.Core.Data;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a simulator that simulates a Game Boy game.
    /// </summary>
    public interface ISimulator
    {
    }

    /// <summary>
    /// Represents a simulator that simulates a specific Game Boy game.
    /// </summary>
    public interface ISimulator<T> : ISimulator where T : class, IGameState
    {
        /// <summary>
        /// The current game state of the simulated Game Boy game.
        /// </summary>
        T GameState { get; }

        /// <summary>
        /// Simulates a command.
        /// </summary>
        /// <param name="command">The command to simulate</param>
        void Simulate(ICommand command);
    }
}
