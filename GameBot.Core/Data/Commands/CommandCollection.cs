using GameBot.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Core.Data.Commands
{
    public class CommandCollection : IEnumerable<ICommand>
    {
        private const double defaultTimeDelta = 0.1;

        private readonly IList<ICommand> commands;
        private readonly double timeDelta;

        public CommandCollection(double timeDelta = defaultTimeDelta)
        {
            this.commands = new List<ICommand>();
            this.timeDelta = timeDelta;
        }

        public CommandCollection(IEnumerable<ICommand> commands, double timeDelta = defaultTimeDelta)
        {
            this.commands = commands.ToList();
            this.timeDelta = timeDelta;
        }

        public void Add(ICommand command)
        {
            commands.Add(command);
        }

        public void AddRange(IEnumerable<ICommand> commands)
        {
            foreach (var command in commands)
            {
                Add(command);
            }
        }
        
        public ICommand Pop()
        {
            if (commands.Any())
            {
                var first = commands.First();
                commands.RemoveAt(0);
                return first;
            }
            return null;
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

        public void HitDelta(Button button)
        {
            TimeSpan timestamp = TimeSpan.Zero;
            if (commands.Any())
            {
                timestamp = commands.Last().Timestamp.Add(TimeSpan.FromSeconds(timeDelta));
            }
            commands.Add(new HitCommand(button, timestamp));
        }

        public void Press(Button button)
        {
            commands.Add(new PressCommand(button));
        }

        public void Press(Button button, double timestamp)
        {
            commands.Add(new PressCommand(button, timestamp.ToTimestamp()));
        }

        public void Press(Button button, TimeSpan timestamp)
        {
            commands.Add(new PressCommand(button, timestamp));
        }

        public void Release(Button button)
        {
            commands.Add(new ReleaseCommand(button));
        }

        public void Release(Button button, double timestamp)
        {
            commands.Add(new ReleaseCommand(button, timestamp.ToTimestamp()));
        }

        public void Release(Button button, TimeSpan timestamp)
        {
            commands.Add(new ReleaseCommand(button, timestamp));
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
