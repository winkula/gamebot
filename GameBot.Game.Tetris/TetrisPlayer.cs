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
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris
{
    public class TetrisPlayer : IPlayer<TetrisGameState>
    {
        private readonly IConfig config;
        private readonly ISearch<TetrisNode> search;
        
        public TetrisGameState CurrentGameState { get; private set; }
        public Move LastMove { get; private set; }

        public TetrisPlayer(IConfig config)
        {
            this.config = config;

            this.search = new TetrisSearch(BuildHeuristic());

            CurrentGameState = new TetrisGameState();
            CurrentGameState.StartLevel = config.Read("Game.Tetris.StartLevel", 0);
            CurrentGameState.Piece = null;
            CurrentGameState.NextPiece = null;
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
            if (gameState == null) throw new ArgumentNullException(nameof(gameState));
            if (gameState.Piece == null) throw new ArgumentNullException(nameof(gameState.Piece));
            if (gameState.NextPiece == null) throw new ArgumentNullException(nameof(gameState.NextPiece));

            var commands = new CommandCollection();

            // release down key if still pressed
            commands.Add(new ReleaseCommand(Button.Down));

            // update current game state
            CurrentGameState.Piece = gameState.Piece;
            CurrentGameState.NextPiece = gameState.NextPiece;

            var start = new TetrisNode(new TetrisGameState(CurrentGameState));
            var result = search.Search(start);
            var move = result?.Parent.Move;
            if (move != null)
            {
                if (move.Rotation % 4 == 3)
                {
                    // counterclockwise rotation
                    commands.Hit(Button.B);
                    CurrentGameState.RotateCounterclockwise();
                }
                else
                {
                    // clockwise rotation
                    Enumerable.Range(0, move.Rotation % 4)
                        .ToList()
                        .ForEach(x =>
                        {
                            commands.Hit(Button.A);
                            CurrentGameState.Rotate();
                        });
                }

                if (move.Translation < 0)
                {
                    // move left
                    Enumerable.Range(0, -move.Translation)
                        .ToList()
                        .ForEach(x =>
                        {
                            commands.Hit(Button.Left);
                            CurrentGameState.Left();
                        });
                }
                else if (move.Translation > 0)
                {
                    // move right
                    Enumerable.Range(0, move.Translation)
                        .ToList()
                        .ForEach(x =>
                        {
                            commands.Hit(Button.Right);
                            CurrentGameState.Right();
                        });
                }

                // drop
                commands.Add(new PressCommand(Button.Down));
                CurrentGameState.Drop(gameState.NextPiece.Value);
                LastMove = move;

                Debug.WriteLine(CurrentGameState);
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
            SelectLevel(commands, CurrentGameState.StartLevel);
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
