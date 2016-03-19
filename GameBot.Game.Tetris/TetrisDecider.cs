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
                double waitingTime = 5.0;
                
                // start 1 player mode
                commands.Add(Button.Start, waitingTime);

                // choose a-type
                commands.Add(Button.A, waitingTime + 1);

                // switch music off
                commands.Add(Button.Right, waitingTime + 2);
                commands.Add(Button.Down, waitingTime + 2.5);
                commands.Add(Button.A, waitingTime + 3);

                // choose level
                commands.Add(Button.Right, waitingTime + 4);
                commands.Add(Button.Right, waitingTime + 4.5);
                commands.Add(Button.A, waitingTime + 5);

                started = true;
            }
            return commands;
        }
    }
}
