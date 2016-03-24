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
        private readonly ISearch<TetrisNode> tetrisSearch = new TetrisSearch(new TetrisHeuristic());

        private bool started = false;

        private TetrisGameState lastGameState;

        public ICommands Decide(TetrisGameStateFull gameState, IContext<TetrisGameStateFull> context)
        {
            var commands = new Commands();
            if (!started)
            {
                // Initialization mode

                //Start(commands);
                QuickStart(commands);

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
                    var start = new TetrisNode(new TetrisGameState(gameState.State));
                    var result = tetrisSearch.Search(start);

                    var goal = result.Parent;
                    var move = result.Parent.Move;
                    
                    Debug.WriteLine("======= Goal state ======= ");
                    Debug.WriteLine(move);
                    Debug.WriteLine(goal);
                    Debug.WriteLine("========================== ");

                    for (int i = 0; i < move.Rotation; i++)
                    {
                        commands.Add(Button.A);
                    }

                    if (move.Translation < 0)
                    {
                        for (int i = 0; i < -move.Translation; i++)
                        {
                            commands.Add(Button.Left);
                        }
                    }
                    else if (move.Translation > 0)
                    {
                        for (int i = 0; i < move.Translation; i++)
                        {
                            commands.Add(Button.Right);
                        }
                    }

                    if (move.Fall > 0)
                    {
                        int slip = 0;//= fall*3/4;
                        for (int i = 0; i < move.Fall - slip; i++)
                        {
                            commands.Add(Button.Down);
                        }
                    }

                    lastGameState = new TetrisGameState(gameState.State);
                }
            }
            return commands;
        }

        private void Start(Commands commands)
        {
            // skip credits
            double waitingTime = 6.0;

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
        }

        private void QuickStart(Commands commands)
        {
            // skip credits
            double waitingTime = 5.0;

            // start 1 player mode
            commands.Add(Button.Start, waitingTime);

            // choose a-type
            commands.Add(Button.A, waitingTime + 1);

            // choose music
            commands.Add(Button.A, waitingTime + 1.5);

            // choose level
            commands.Add(Button.A, waitingTime + 2);
        }
    }
}
