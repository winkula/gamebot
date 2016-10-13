using GameBot.Core.Exceptions;
using GameBot.Game.Tetris.Searching;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace GameBot.Game.Tetris.Simulator
{
    public class SimulatorEngine
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ISearch search;
        private readonly TetrisSimulator simulator;

        // in ms
        public int PauseTime { get; set; }

        public SimulatorEngine(ISearch search, TetrisSimulator simulator)
        {
            this.search = search;
            this.simulator = simulator;
        }

        public void Run()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int round = 0;
            while (true)
            {
                try
                {
                    if (round % 600 == 0)
                    {
                        WriteStatus(round, ">..");
                    }
                    else if (round % 600 == 100)
                    {
                        WriteStatus(round, ".>.");
                    }
                    else if (round % 600 == 200)
                    {
                        WriteStatus(round, "..>");
                    }
                    else if (round % 600 == 300)
                    {
                        WriteStatus(round, "..<");
                    }
                    else if (round % 600 == 400)
                    {
                        WriteStatus(round, ".<.");
                    }
                    else if (round % 600 == 500)
                    {
                        WriteStatus(round, "<..");
                    }

                    Update();
                    logger.Info($"Round {round + 1}");
                    Render();

                    round++;
                }
                catch (GameOverException)
                {
                    LogResults(round, stopwatch.ElapsedMilliseconds);
                    break;
                }
            }
            stopwatch.Stop();
        }

        private void WriteStatus(int round, string animation)
        {
            Console.Clear();
            Console.WriteLine($"Running {animation}");
            Console.WriteLine($"Round {round}");
            Console.WriteLine($"---------------------");
            Console.WriteLine($"Level {simulator.GameState.Level}");
            Console.WriteLine($"Score {simulator.GameState.Score}");
            Console.WriteLine($"Lines {simulator.GameState.Lines}");
        }

        protected void Update()
        {
            var result = search.Search(simulator.GameState);
            if (result != null && result.Moves.Any())
            {
                foreach (var move in result.Moves)
                {
                    simulator.Simulate(move);
                }
            }
        }

        protected void Render()
        {
            logger.Info(simulator.GameState);
            if (PauseTime > 0)
            {
                Thread.Sleep(PauseTime);
            }
        }

        protected void LogResults(int rounds, long time)
        {
            logger.Error("Game over");
            Console.WriteLine("Game over");

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Simulator_Results.csv");

            if (!File.Exists(path))
            {
                using (var writer = File.CreateText(path))
                {
                    writer.WriteLine("Rounds,Lines,Score,Level,Time");
                }
            }

            string message = $"{rounds},{simulator.GameState.Lines},{simulator.GameState.Score},{simulator.GameState.Level},{time}\n";
            File.AppendAllText(path, message);
        }
    }
}