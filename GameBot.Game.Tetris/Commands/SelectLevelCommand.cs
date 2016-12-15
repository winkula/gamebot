using System;
using GameBot.Core;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris.Commands
{
    public class SelectLevelCommand : ICommand
    {
        private static readonly TimeSpan _buttonWaitDuration = TimeSpan.FromMilliseconds(100);

        private readonly IExecutor _executor;
        private readonly int _level;

        public SelectLevelCommand(IExecutor executor, int level)
        {
            if (executor == null) throw new ArgumentNullException(nameof(executor));
            if (level < 0 || level > 9) throw new ArgumentException("level must be between 0 and 9 (inclusive)");

            _executor = executor;
            _level = level;
        }

        public void Execute()
        {
            Align();
            SelectLevel();
        }

        private void Align()
        {
            _executor.HitWait(Button.Up, _buttonWaitDuration);

            for (int i = 0; i < 4; i++)
            {
                _executor.HitWait(Button.Left, _buttonWaitDuration);
            }
        }

        private void SelectLevel()
        {
            if (_level >= 5)
            {
                _executor.HitWait(Button.Down, _buttonWaitDuration);
            }
            for (int i = 0; i < _level % 5; i++)
            {
                _executor.HitWait(Button.Right, _buttonWaitDuration);
            }
        }
    }
}
