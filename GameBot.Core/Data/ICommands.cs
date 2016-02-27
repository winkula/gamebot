using System.Collections.Generic;

namespace GameBot.Core.Data
{
    /// <summary>
    /// Represents a collection of commands to the Game Boy device.
    /// </summary>
    public interface ICommands : IEnumerable<ICommand>
    {
    }
}
