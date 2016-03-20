using System;
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

        public void Add(ICommand command)
        {
            commands.Add(command);
        }

        public void Add(Button button)
        {
            commands.Add(new Command(button));
        }

        public void Add(Button button, double timestamp)
        {
            commands.Add(new Command(button, timestamp.ToTimestamp()));
        }

        public void Add(Button button, TimeSpan timestamp)
        {
            commands.Add(new Command(button, timestamp));
        }

        public void Add(Button button, TimeSpan timestamp, TimeSpan duration)
        {
            commands.Add(new Command(button, timestamp, duration));
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
