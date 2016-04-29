﻿using GameBot.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Core.Data.Commands
{
    public class CommandCollection : IEnumerable<ICommand>
    {
        private readonly IList<ICommand> commands;
        private readonly double timeDelta = 0.1;

        public CommandCollection(double timeDelta = 0.1)
        {
            this.commands = new List<ICommand>();
            this.timeDelta = timeDelta;
        }

        public CommandCollection(IEnumerable<ICommand> commands , double timeDelta = 0.1)
        {
            this.commands = commands.ToList();
            this.timeDelta = timeDelta;
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
