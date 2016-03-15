using GameBot.Core;
using System;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris
{
    public class TetrisDecider : IDecider<TetrisGameStateFull>
    {
        public ICommands Decide(TetrisGameStateFull gameState, IContext<TetrisGameStateFull> context)
        {
            var commands = new Commands();

            if (new Random().NextDouble() < 0.01)
            {
                commands.AddCommand(new Command(Button.Start, new TimeSpan(), new TimeSpan()));
            }

            return commands;
        }
    }
}
