using GameBot.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Core.Data.Commands
{
    public class CommandCollection : IEnumerable<ICommand>
    {
        private const double DefaultTimeDelta = 0.1;

        private readonly IList<ICommand> _commands;
        private readonly double _timeDelta;

        public CommandCollection(double timeDelta = DefaultTimeDelta)
        {
            _commands = new List<ICommand>();
            _timeDelta = timeDelta;
        }

        public CommandCollection(IEnumerable<ICommand> commands, double timeDelta = DefaultTimeDelta)
        {
            _commands = commands.ToList();
            _timeDelta = timeDelta;
        }

        public void Add(ICommand command)
        {
            _commands.Add(command);
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
            if (_commands.Any())
            {
                var first = _commands.First();
                _commands.RemoveAt(0);
                return first;
            }
            return null;
        }

        public void Hit(Button button)
        {
            _commands.Add(new HitCommand(button));
        }

        public void Hit(Button button, double timestamp)
        {
            _commands.Add(new HitCommand(button, timestamp.ToTimestamp()));
        }

        public void Hit(Button button, TimeSpan timestamp)
        {
            _commands.Add(new HitCommand(button, timestamp));
        }

        public void HitDelta(Button button)
        {
            TimeSpan timestamp = TimeSpan.Zero;
            if (_commands.Any())
            {
                timestamp = _commands.Last().Timestamp.Add(TimeSpan.FromSeconds(_timeDelta));
            }
            _commands.Add(new HitCommand(button, timestamp));
        }

        public void Press(Button button)
        {
            _commands.Add(new PressCommand(button));
        }

        public void Press(Button button, double timestamp)
        {
            _commands.Add(new PressCommand(button, timestamp.ToTimestamp()));
        }

        public void Press(Button button, TimeSpan timestamp)
        {
            _commands.Add(new PressCommand(button, timestamp));
        }

        public void Release(Button button)
        {
            _commands.Add(new ReleaseCommand(button));
        }

        public void Release(Button button, double timestamp)
        {
            _commands.Add(new ReleaseCommand(button, timestamp.ToTimestamp()));
        }

        public void Release(Button button, TimeSpan timestamp)
        {
            _commands.Add(new ReleaseCommand(button, timestamp));
        }

        public IEnumerator<ICommand> GetEnumerator()
        {
            return _commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _commands.GetEnumerator();
        }
    }
}
