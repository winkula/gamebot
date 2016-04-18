using GameBot.Core;
using System.Collections.Generic;
using System.Linq;
using GameBot.Core.Data;
using GameBot.Emulation;
using System;

namespace GameBot.Robot.Executors
{
    public class EmulatorExecutor : IExecutor
    {
        private readonly Emulator emulator;
        private readonly IActuator actuator;
        private readonly ITimeProvider timeProvider;
        private readonly List<ICommand> queue;

        public EmulatorExecutor(Emulator emulator, IActuator actuator, ITimeProvider timeProvider)
        {
            this.emulator = emulator;
            this.timeProvider = timeProvider;
            this.actuator = actuator;
            this.queue = new List<ICommand>();
        }

        public void Execute(IEnumerable<ICommand> commands)
        {
            queue.AddRange(commands);            
            Execute();
        }

        public void Execute(ICommand command)
        {
            queue.Add(command);
            Execute();
        }

        private void Execute()
        {
            var time = timeProvider.Time;

            var pendingCommands = queue.Where(x => IsPending(x, time)).ToList();

            foreach (var pendingCommand in pendingCommands)
            {
                pendingCommand.Execute(emulator);
            }

            queue.RemoveAll(x => IsPending(x, time));

            // emulate one frame
            //emulator.ExecuteFrame();
            emulator.ExecuteFrames(3);
        }

        private bool IsPending(ICommand command, TimeSpan time) { return command.Timestamp <= time; }
    }
}
