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
        private readonly IActor actor;
        private readonly ITimeProvider timeProvider;
        private readonly List<ICommand> queue;

        public EmulatorExecutor(Emulator emulator, IActor actor, ITimeProvider timeProvider)
        {
            this.emulator = emulator;
            this.timeProvider = timeProvider;
            this.actor = actor;
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

            var pending = queue.Where(x =>
                IsHitCommand(x, time) ||
                IsPressCommand(x, time) ||
                IsReleaseCommand(x, time)).ToList();

            foreach (var command in pending)
            {
                if (IsHitCommand(command, time)) { emulator.HitButton(command.Button); }
                else if (IsPressCommand(command, time)) { emulator.PressButton(command.Button); }
                else if (IsReleaseCommand(command, time)) { emulator.ReleaseButton(command.Button); }
            }

            queue.RemoveAll(x =>
                IsHitCommand(x, time) ||
                IsPressCommand(x, time) ||
                IsReleaseCommand(x, time));

            // emulate one frame
            //emulator.ExecuteFrame();
            emulator.ExecuteFrames(3);
        }

        private bool IsHitCommand(ICommand command, TimeSpan time) { return command.Press <= time && command.Release != null; }
        private bool IsPressCommand(ICommand command, TimeSpan time) { return command.Press <= time && command.Release == null; }
        private bool IsReleaseCommand(ICommand command, TimeSpan time) { return command.Press == null && command.Release <= time; }
    }
}
