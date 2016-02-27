using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Core.Data
{
    public class Commands : ICommands
    {
        private readonly IList<ICommand> commands;

        public Commands()
        {
            commands = new List<ICommand>();
        }

        public Commands(IEnumerable<ICommand> commands)
        {
            this.commands = commands.ToList();
        }

        public void AddCommand(ICommand command)
        {
            commands.Add(command);
        }

        public IEnumerator<ICommand> GetEnumerator()
        {
            return commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return commands.GetEnumerator();
        }
    }
}
