using GameBot.Core.Data;
using System;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a handler for commands to the Game Boy device.
    /// </summary>
    public interface IExecutor
    {
        /// <summary>
        /// Executes a command on the Game Boy.
        /// </summary>
        /// <param name="command">A command to the Game Boy.</param>
        /// <param name="timestamp">The current timestamp.</param>
        void Execute(ICommand command, TimeSpan timestamp);

        /// <summary>
        /// Executes a collection of commands on the Game Boy.
        /// </summary>
        /// <param name="commands">A collection of commands to the Game Boy.</param>
        /// <param name="timestamp">The current timestamp.</param>
        void Execute(ICommands commands, TimeSpan timestamp);
    }
}
