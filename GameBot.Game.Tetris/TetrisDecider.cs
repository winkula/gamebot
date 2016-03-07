using GameBot.Core;
using System;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris
{
    public class TetrisDecider : IDecider<TetrisGameState>
    {
        public ICommands Decide(TetrisGameState gameState, IContext<TetrisGameState> context)
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
