using GameBot.Core;
using System;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris
{
    public class TetrisDecider : IDecider<TetrisGameStateFull>
    {
        private bool started = false;

        public ICommands Decide(TetrisGameStateFull gameState, IContext<TetrisGameStateFull> context)
        {
            var commands = new Commands();
            if (!started)
            {
                // Initialization

                // skip credits
                commands.Add(Button.Start, 4.0);
                // start 1 player mode
                commands.Add(Button.Start, 4.5);

                // choose a-type
                commands.Add(Button.A, 5.0);

                // switch music off
                commands.Add(Button.Right, 6.0);
                commands.Add(Button.Down, 6.5);
                commands.Add(Button.A, 7.0);

                // choose level
                commands.Add(Button.Right, 8.0);
                commands.Add(Button.Right, 8.5);
                commands.Add(Button.A, 9.0);

                started = true;
            }
            return commands;
        }
    }
}
