using System;
using GameBot.Core;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris.Commands
{
    public class HeartModeCommand : ICommand
    {
        private static readonly TimeSpan _buttonWaitDuration = TimeSpan.FromMilliseconds(100);

        private readonly IExecutor _executor;
        private readonly bool _heartMode;

        public HeartModeCommand(IExecutor executor, bool heartMode)
        {
            if (executor == null) throw new ArgumentNullException(nameof(executor));
            
            _executor = executor;
            _heartMode = heartMode;
        }

        public void Execute()
        {
            StartFromMenu();
        }

        private void StartFromMenu()
        {
            if (_heartMode) _executor.Press(Button.Down);

            // start 1 player mode
            _executor.HitWait(Button.Start, _buttonWaitDuration);

            if (_heartMode) _executor.Release(Button.Down);

            // choose a-type
            _executor.HitWait(Button.A, _buttonWaitDuration);

            // choose music
            _executor.HitWait(Button.Right, _buttonWaitDuration);
            _executor.HitWait(Button.Down, _buttonWaitDuration);
            _executor.HitWait(Button.A, _buttonWaitDuration);
        }
    }
}
