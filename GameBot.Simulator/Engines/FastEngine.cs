using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using GameBot.Core.Exceptions;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace GameBot.Robot.Engines
{
    public class FastEngine : IEngine
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ISearch search;
        private readonly TetrisSimulator simulator;

        public FastEngine(ISearch search, TetrisSimulator simulator)
        {
            this.search = search;
            this.simulator = simulator;
        }

        public void Run()
        {
            Init();
            Loop();
        }

        protected void Init()
        {
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
            var result = search.Search(simulator.GameState);
            if (result != null && result.Moves.Any())
            {
                foreach (var move in result.Moves)
                {
                    simulator.Simulate(move);
                    Render();
                }
            }
            else
            {
                // simulate fall
                Render();
            }
        }

        protected void Render()
        {
            logger.Info("Current game state:\n" + simulator.GameState);
            Thread.Sleep(1000);
        }

        protected void Log(int rounds, long time)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "/game_log.csv");

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