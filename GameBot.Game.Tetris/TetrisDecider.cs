using GameBot.Core;
using System;
using GameBot.Core.Data;
using GameBot.Core.Searching;
using System.Diagnostics;

namespace GameBot.Game.Tetris
{
    public class TetrisDecider : IDecider<TetrisGameStateFull>
    {
        private readonly DepthFirstSearch search = new DepthFirstSearch();

        private bool started = false;

        private TetrisGameState lastGameState;

        public ICommands Decide(TetrisGameStateFull gameState, IContext<TetrisGameStateFull> context)
        {
            var commands = new Commands();
            if (!started)
            {
                // Initialization mode

                // skip credits
                double waitingTime = 5.0;

                /*
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
                */

                // start 1 player mode
                commands.Add(Button.Start, waitingTime);

                // choose a-type
                commands.Add(Button.A, waitingTime + 1);

                // choose music
                commands.Add(Button.A, waitingTime + 1.5);

                // choose level
                commands.Add(Button.A, waitingTime + 2);

                started = true;
            }
            else
            {
                // Play mode

                if (
                    gameState.State.Piece != null &&
                    gameState.State.NextPiece != null &&
                    (lastGameState == null || !lastGameState.Equals(gameState.State))
                    )
                {
                    var node = new TetrisNode(gameState.State);
                    var winner = (TetrisNode)search.Search(node);
                    
                    Debug.WriteLine(winner.Parent);


                    var orientation = ((TetrisNode)(winner.Parent)).Orientation;
                    var translation = ((TetrisNode)(winner.Parent)).Translation;
                    var fall = ((TetrisNode)(winner.Parent)).Fall;

                    Debug.WriteLine("======= Act! ======= ");
                    Debug.WriteLine("Orientation: " + orientation);
                    Debug.WriteLine("Translation: " + translation);
                    Debug.WriteLine("Fall: " + fall);

                    for (int i = 0; i < orientation; i++)
                    {
                        commands.Add(Button.A);
                    }

                    if (translation < 0)
                    {
                        for (int i = 0; i < -translation; i++)
                        {
                            commands.Add(Button.Left);
                        }
                    }
                    else if (translation > 0)
                    {
                        for (int i = 0; i < translation; i++)
                        {
                            commands.Add(Button.Right);
                        }
                    }

                    /*
                    if (fall > 0)
                    {
                        for (int i = 0; i < fall; i++)
                        {
                            commands.Add(Button.Down);
                        }
                    }
                    */

                    lastGameState = gameState.State;
                }

            }
            return commands;
        }
    }
}
