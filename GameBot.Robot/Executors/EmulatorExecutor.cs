using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using GameBot.Core.Data;
using GameBot.Emulation;

namespace GameBot.Robot.Executors
{
    public class EmulatorExecutor : IExecutor
    {
        private readonly Emulator emulator;
        private readonly List<ICommand> queue;

        public EmulatorExecutor(Emulator emulator)
        {
            this.emulator = emulator;
            this.queue = new List<ICommand>();
        }

        public void Execute(ICommands commands, TimeSpan timestamp)
        {
            foreach (var command in commands)
            {
                queue.Add(command);
            }
            Execute(timestamp);
        }

        public void Execute(ICommand command, TimeSpan timestamp)
        {
            queue.Add(command);
            Execute(timestamp);
        }

        private void Execute(TimeSpan timestamp)
        {
            var pending = queue.Where(x => x.Timestamp <= timestamp).ToList();
            foreach (var ex in pending)
            {
                emulator.KeyTyped(ex.Button);
            }

            queue.RemoveAll(x => x.Timestamp <= timestamp);

            // next frame
            emulator.ExecuteFrame();
        }
    }
}
