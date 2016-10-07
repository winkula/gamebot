﻿using GameBot.Core;
using System;
using GameBot.Core.Data;
using System.Diagnostics;
using System.Collections.Generic;
using GameBot.Core.Data.Commands;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Searching.Heuristics;

namespace GameBot.Game.Tetris
{
    public class TetrisAi
    {
        private readonly IConfig config;
        private readonly ISearch search;

        public GameState CurrentGameState { get; private set; }
        public Way LastWay { get; private set; }

        public TetrisAi(IConfig config)
        {
            this.config = config;

            this.search = new SimpleSearch(BuildHeuristic());

            CurrentGameState = new GameState();
            CurrentGameState.StartLevel = config.Read("Game.Tetris.StartLevel", 0);
            CurrentGameState.Piece = null;
            CurrentGameState.NextPiece = null;
        }

        private IHeuristic BuildHeuristic()
        {
            var typeName = config.Read("Game.Tetris.Heuristic", "GameBot.Game.Tetris.Searching.Heuristics.YiyuanLeeHeuristic");
            var type = Type.GetType(typeName);
            return (IHeuristic)Activator.CreateInstance(type);
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

        public IEnumerable<ICommand> Play(GameState gameState)
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
            
            var result = search.Search(new GameState(CurrentGameState));

            foreach (var move in result.Moves)
            {
                switch (move)
                {
                    case Move.Left:
                        // move left
                        commands.Hit(Button.Left);
                        CurrentGameState.Left();
                        break;

                    case Move.Right:
                        // move right
                        commands.Hit(Button.Right);
                        CurrentGameState.Right();
                        break;

                    case Move.Rotate:
                        // clockwise rotation
                        commands.Hit(Button.A);
                        CurrentGameState.Rotate();
                        break;

                    case Move.RotateCounterclockwise:
                        // counterclockwise rotation
                        commands.Hit(Button.B);
                        CurrentGameState.RotateCounterclockwise();
                        break;

                    case Move.Drop:
                        // drop
                        commands.Add(new PressCommand(Button.Down));
                        CurrentGameState.Drop(gameState.NextPiece.Value);
                        LastWay = result.Way;
                        break;

                    default:
                        break;
                }
            }

            Debug.WriteLine(CurrentGameState);
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
