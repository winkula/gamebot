using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using GameBot.Core.Data;
using GameBot.Emulation;
using System.Diagnostics;

namespace GameBot.Robot.Executors
{
    public class EmulatorExecutor : IExecutor
    {
        private readonly Emulator emulator;
        private readonly List<ICommand> queue;
        public ITimeProvider TimeProvider { private get; set; }

        public EmulatorExecutor(Emulator emulator)
        {
            this.emulator = emulator;
            this.queue = new List<ICommand>();
        }

        public void Execute(ICommands commands)
        {
            foreach (var command in commands)
            {
                queue.Add(command);
            }
            Execute();
        }

        public void Execute(ICommand command)
        {
            queue.Add(command);
            Execute();
        }

        private void Execute()
        {
            var time = TimeProvider.Time;

            var pending = queue.Where(x => x.Press <= time).ToList();
            var buttons = pending.Select(x => x.Button).ToList();
            
            emulator.KeysTyped(buttons);
            if (buttons.Any())
            {
                Debug.WriteLine("Press key " + string.Join(", ", buttons));
            }

            queue.RemoveAll(x => x.Press <= time);

            // emulate one frame
            emulator.ExecuteFrame();
        }
    }
}
