﻿using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using GameBot.Core.Exceptions;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GameBot.Robot.Engines
{
    public class FastEngine : IEngine
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IPlayer<GameState> player;
        private readonly TetrisSimulator simulator;

        public FastEngine(IPlayer<GameState> player, TetrisSimulator emulator)
        {
            this.player = player;
            this.simulator = emulator;
        }

        public void Run()
        {
            Init();
            Loop();
        }

        protected void Init()
        {
            // first call to play only handles the menu actions
            player.Play(simulator.GameState);
        }

        protected void Loop()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int round = 0;
            while (true)
            {
                try
                {
                    // status log
                    if (round % 100 == 0) logger.Info("Play round " + round + "...");

                    Update();
                    //Render();

                    round++;
                }
                catch (GameOverException)
                {
                    Log(round, stopwatch.ElapsedMilliseconds);
                    break;
                }
            }
            stopwatch.Stop();
        }

        protected void Update()
        {
            IEnumerable<ICommand> commands = player.Play(simulator.GameState);
            if (commands.Any())
            {
                foreach (var command in commands)
                {
                    simulator.Simulate(command);
                    Render();
                }
            }
            else
            {
                // simulate fall
                simulator.Simulate(new HitCommand(Button.Down));
                Render();
            }
        }

        protected void Render()
        {
            //logger.Info(simulator.GameState);
            //Thread.Sleep(200);
        }

        protected void Log(int rounds, long time)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/game_log.csv";

            if (!File.Exists(path))
            {
                //File.Create(path);
                File.AppendAllText(path, "Rounds,Lines,Score,Level,Time\n");
            }

            string message = $"{rounds},{simulator.GameState.Lines},{simulator.GameState.Score},{simulator.GameState.Level},{time}\n";
            File.AppendAllText(path, message);
        }
        
        public void Initialize()
        {
            throw new NotSupportedException("Only the 'run' mode is supported.");
        }

        public void Step(bool play, Action<IImage, IImage> callback)
        {
            throw new NotSupportedException("Only the 'run' mode is supported.");
        }
    }
}