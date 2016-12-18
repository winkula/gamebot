using System;
using GameBot.Core;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris.Commands
{
    public class StartFromGameOverCommand : ICommand
    {
        private static readonly TimeSpan _buttonWaitDuration = TimeSpan.FromMilliseconds(100);

        private readonly IExecutor _executor;

        public StartFromGameOverCommand(IExecutor executor)
        {
            if (executor == null) throw new ArgumentNullException(nameof(executor));
            
            _executor = executor;
        }

        public void Execute()
        {
            // sequence handles both cases (with and without entry in high score table)

            _executor.HitWait(Button.Start, _buttonWaitDuration);
            _executor.HitWait(Button.B, _buttonWaitDuration);
            _executor.HitWait(Button.Start, _buttonWaitDuration);
            _executor.Hit(Button.Start);
        }
    }
}
