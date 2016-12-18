using System;
using System.Linq;
using GameBot.Core;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris.Commands
{
    public class HighscoreCommand : ICommand
    {
        private const string _validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ.-+_";

        private static readonly TimeSpan _buttonWaitDuration = TimeSpan.FromMilliseconds(100);

        private readonly IExecutor _executor;
        private readonly string _name;

        public HighscoreCommand(IExecutor executor, string name)
        {
            if (executor == null) throw new ArgumentNullException(nameof(executor));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (name.Length > 6) throw new ArgumentException("name must not have more than 6 characters");
            if (!name.All(c => _validChars.Contains(c))) throw new ArgumentException($"valid characters are: {_validChars}");

            _executor = executor;
            _name = name;
        }

        public void Execute()
        {
            Write();
        }

        private void Write()
        {
            bool first = true;

            foreach (var character in _name)
            {
                if (!first)
                {
                    _executor.HitWait(Button.A, _buttonWaitDuration);
                }

                var distUp = DistanceUp(character);
                var distDown = DistanceDown(character);

                if (distUp <= distDown)
                {
                    Up(distUp);
                }
                else
                {
                    Down(distDown);
                }

                first = false;
            }

            if (_name.Length < 6)
            {
                _executor.Hit(Button.Start);
            }
        }

        private int DistanceUp(char character)
        {
            return _validChars.IndexOf(character);
        }

        private int DistanceDown(char character)
        {
            return _validChars.Length - _validChars.IndexOf(character);
        }

        private void Up(int dist)
        {
            for (int i = 0; i < dist; i++)
            {
                _executor.HitWait(Button.Up, _buttonWaitDuration);
            }
        }

        private void Down(int dist)
        {
            for (int i = 0; i < dist; i++)
            {
                _executor.HitWait(Button.Down, _buttonWaitDuration);
            }
        }
    }
}
