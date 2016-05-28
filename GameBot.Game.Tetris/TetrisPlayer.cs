using GameBot.Core;
using System;
using GameBot.Core.Data;
using GameBot.Core.Searching;
using System.Diagnostics;
using System.Collections.Generic;
using GameBot.Core.Data.Commands;
using System.Linq;
using System.Configuration;
using GameBot.Game.Tetris.Heuristics;

namespace GameBot.Game.Tetris
{
    public class TetrisPlayer : IPlayer<TetrisGameState>
    {
        private readonly IConfig config;

        private readonly ISearch<TetrisNode> search;
        private int startLevel;

        private TetrisGameState lastGameState;

        public TetrisPlayer(IConfig config)
        {
            this.config = config;

            this.search = new TetrisSearch(BuildHeuristic());
            this.startLevel = config.Read("Game.Tetris.StartLevel", 0);
        }

        private IHeuristic<TetrisGameState> BuildHeuristic()
        {
            var typeName = config.Read("Game.Tetris.Heuristic", "GameBot.Game.Tetris.Heuristics.YiyuanLeeHeuristic");
            var type = Type.GetType(typeName);
            return (IHeuristic<TetrisGameState>)Activator.CreateInstance(type);
        }

        public IEnumerable<ICommand> Initialize()
        {
            var commands = new CommandCollection();
            if (config.Read<bool>("Game.Tetris.Initialize"))
            {
                Start(commands);
            }
            return commands;
        }

        public IEnumerable<ICommand> Play(TetrisGameState gameState)
        {
            var commands = new CommandCollection();
            if (gameState.Piece != null && gameState.NextPiece != null && (lastGameState == null || !lastGameState.Equals(gameState)))
            {
                // release down key if still pressed
                commands.Add(new ReleaseCommand(Button.Down));

                var start = new TetrisNode(new TetrisGameState(gameState));
                var result = search.Search(start);
                var move = result?.Parent.Move;
                if (move != null)
                {
                    if (move.Rotation % 4 == 3)
                    {
                        // counterclockwise rotation
                        commands.Hit(Button.B);
                    }
                    else
                    {
                        // clockwise rotation
                        Enumerable.Range(0, move.Rotation % 4)
                            .ToList()
                            .ForEach(x => commands.Hit(Button.A));
                    }

                    if (move.Translation < 0)
                    {
                        // move left
                        Enumerable.Range(0, -move.Translation)
                            .ToList()
                            .ForEach(x => commands.Hit(Button.Left));
                    }
                    else if (move.Translation > 0)
                    {
                        // move right
                        Enumerable.Range(0, move.Translation)
                            .ToList()
                            .ForEach(x => commands.Hit(Button.Right));
                    }

                    // drop
                    commands.Add(new PressCommand(Button.Down));
                }

                lastGameState = new TetrisGameState(gameState);
            }
            return commands;
        }

        private void Start(CommandCollection commands)
        {
            // skip credits
            double waitingTime = 2.2 + new Random().NextDouble();

            // start 1 player mode
            commands.Hit(Button.Start, waitingTime);

            // choose a-type
            commands.HitDelta(Button.A);

            // choose music
            commands.HitDelta(Button.Right);
            commands.HitDelta(Button.Down);
            commands.HitDelta(Button.A);

            // select level
            SelectLevel(commands, startLevel);
        }

        private void SelectLevel(CommandCollection commands, int startLevel)
        {
            if (startLevel >= 5)
            {
                commands.HitDelta(Button.Down);
            }
            for (int i = 0; i < (startLevel % 5); i++)
            {
                commands.HitDelta(Button.Right);
            }
            commands.HitDelta(Button.A);
        }
    }
}
