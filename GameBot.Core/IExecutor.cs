using GameBot.Core.Data;
using System.Collections.Generic;

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
        void Execute(ICommand command);

        /// <summary>
        /// Executes a collection of commands on the Game Boy.
        /// </summary>
        /// <param name="commands">A collection of commands to the Game Boy.</param>
        void Execute(IEnumerable<ICommand> commands);
    }
}
