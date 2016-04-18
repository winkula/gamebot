using System;

namespace GameBot.Core.Data
{
    /// <summary>
    /// Represents a command to the Game Boy device. Can be a button hit, press or release.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the moment in time, when the command must be executed.
        /// </summary>
        TimeSpan Timestamp { get; }

        /// <summary>
        /// Executes the commands on the actuator.
        /// </summary>
        /// <param name="actuator">The actuator.</param>
        void Execute(IActuator actuator);
    }
}
