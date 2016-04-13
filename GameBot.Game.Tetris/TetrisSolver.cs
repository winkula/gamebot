﻿using GameBot.Core;
using System;
using GameBot.Core.Data;
using GameBot.Core.Searching;
using System.Diagnostics;
using System.Collections.Generic;
using GameBot.Core.Data.Commands;

namespace GameBot.Game.Tetris
{
    public class TetrisSolver : ISolver<TetrisGameState>
    {
        //private readonly DepthFirstSearch search = new DepthFirstSearch();
        private readonly ISearch<TetrisNode> search;// = new TetrisSearch(new TetrisHolesHeuristic());

        private bool started = false;

        private TetrisGameState lastGameState;

        public TetrisSolver(ISearch<TetrisNode> search)
        {
            this.search = search;
        }

        public IEnumerable<ICommand> Solve(TetrisGameState gameState)
        {
            var commands = new CommandCollection();
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
                    gameState.Piece != null &&
                    gameState.NextPiece != null &&
                    (lastGameState == null || !lastGameState.Equals(gameState))
                    )
                {
                    // release down key if still pressed
                    commands.Add(new ReleaseCommand(Button.Down));

                    var start = new TetrisNode(new TetrisGameState(gameState));
                    var result = search.Search(start);

                    var goal = result.Parent;
                    var move = result.Parent.Move;

                    //Debug.WriteLine("======= Goal state ======= ");
                    //Debug.WriteLine(move);
                    //Debug.WriteLine(goal);
                    //Debug.WriteLine("========================== ");

                    for (int i = 0; i < move.Rotation; i++)
                    {
                        commands.Hit(Button.A);
                    }

                    if (move.Translation < 0)
                    {
                        for (int i = 0; i < -move.Translation; i++)
                        {
                            commands.Hit(Button.Left);
                        }
                    }
                    else if (move.Translation > 0)
                    {
                        for (int i = 0; i < move.Translation; i++)
                        {
                            commands.Hit(Button.Right);
                        }
                    }

                    if (move.Fall > 0)
                    {
                        // TODO: set start level
                        // Level.GetDuration(0, gameState.Level)
                        commands.Add(new PressCommand(Button.Down));
                        /*
                        // TODO: drop with press duration (level dependent)
                        int slip = 0;//= fall*3/4;
                        for (int i = 0; i < move.Fall - slip; i++)
                        {
                            commands.Hit(Button.Down);
                        }*/
                    }

                    lastGameState = new TetrisGameState(gameState);
                }
            }
            return commands;
        }

        private void Start(CommandCollection commands)
        {
            // skip credits
            double waitingTime = 6.0;

            // start 1 player mode
            commands.Hit(Button.Start, waitingTime);

            // choose a-type
            commands.Hit(Button.A, waitingTime + 1);

            // switch music off
            commands.Hit(Button.Right, waitingTime + 2);
            commands.Hit(Button.Down, waitingTime + 2.5);
            commands.Hit(Button.A, waitingTime + 3);

            // choose level
            commands.Hit(Button.Right, waitingTime + 4);
            commands.Hit(Button.Right, waitingTime + 4.5);
            commands.Hit(Button.A, waitingTime + 5);
        }

        private void QuickStart(CommandCollection commands)
        {
            // skip credits
            double waitingTime = 2.2 + new Random().NextDouble();
            double delta = 0.25;

            // start 1 player mode
            commands.Hit(Button.Start, waitingTime);

            // choose a-type
            commands.Hit(Button.A, waitingTime + 1 * delta);

            // choose music
            commands.Hit(Button.A, waitingTime + 2 * delta);

            // choose level
            commands.Hit(Button.A, waitingTime + 3 * delta);
        }
    }
}
