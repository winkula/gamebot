using GameBot.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Core.Data.Commands
{
    public class CommandCollection : IEnumerable<ICommand>
    {
        private readonly IList<ICommand> commands;

        public CommandCollection()
        {
            commands = new List<ICommand>();
        }

        public CommandCollection(IEnumerable<ICommand> commands)
        {
            this.commands = commands.ToList();
        }

        public void Add(ICommand command)
        {
            commands.Add(command);
        }

        public void Hit(Button button)
        {
            commands.Add(new HitCommand(button));
        }

        public void Hit(Button button, double timestamp)
        {
            commands.Add(new HitCommand(button, timestamp.ToTimestamp()));
        }

        public void Hit(Button button, TimeSpan timestamp)
        {
            commands.Add(new HitCommand(button, timestamp));
        }

        public void Hit(Button button, TimeSpan timestamp, TimeSpan duration)
        {
            commands.Add(new HitCommand(button, timestamp, duration));
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
