using GameBot.Core.Exceptions;
using GameBot.Game.Tetris.Searching;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using GameBot.Game.Tetris.Simulators;

namespace GameBot.Game.Tetris.Simulator
{
    public class SimulatorEngine
    {
        private int _maxHeight = 10;

        private readonly Stopwatch _stopwatch;
        private readonly Stopwatch _stopwatchRound;

        private readonly ISearch _search;
        private readonly TetrisSimulator _simulator;

        // in ms
        public int PauseTime { get; set; }
        public int FrameUpdateDelay { get; set; }
        public bool Multiplayer { get; set; }

        public SimulatorEngine(ISearch search, TetrisSimulator simulator)
        {
            _stopwatch = new Stopwatch();
            _stopwatchRound = new Stopwatch();

            _search = search;
            _simulator = simulator;

            FrameUpdateDelay = 60;
        }

        public void Run()
        {
            _stopwatch.Start();
            _stopwatchRound.Start();
            int round = 0;
            int multiplayerHolePosition = new Random().Next(0, 9);
            while (true)
            {
                int updateEvery = FrameUpdateDelay;
                try
                {
                    if (round % updateEvery == 0)
                    {
                        WriteStatus(round, ">..");
                    }
                    else if (round % updateEvery == 1 * updateEvery / 6)
                    {
                        WriteStatus(round, ".>.");
                    }
                    else if (round % updateEvery == 2 * updateEvery / 6)
                    {
                        WriteStatus(round, "..>");
                    }
                    else if (round % updateEvery == 3 * updateEvery / 6)
                    {
                        WriteStatus(round, "..<");
                    }
                    else if (round % updateEvery == 4 * updateEvery / 6)
                    {
                        WriteStatus(round, ".<.");
                    }
                    else if (round % updateEvery == 5 * updateEvery / 6)
                    {
                        WriteStatus(round, "<..");
                    }

                    Update(round, multiplayerHolePosition);
                    Render();
                    
                    /*
                    if (_simulator.GameState.Board.MaximumHeight >= _maxHeight)
                    {
                        throw new GameOverException();
                    }
                    */

                    round++;
                }
                catch (GameOverException)
                {
                    LogResults(round, _stopwatch.ElapsedMilliseconds);
                    break;
                }
            }
            _stopwatch.Stop();
        }

        private void WriteStatus(int round, string animation)
        {
            Console.Clear();
            Console.WriteLine($@"Running {animation}");
            Console.WriteLine($@"Round {round}");
            Console.WriteLine(@"---------------------");
            Console.WriteLine($@"Level {_simulator.GameState.Level}");
            Console.WriteLine($@"Score {_simulator.GameState.Score}");
            Console.WriteLine($@"Lines {_simulator.GameState.Lines}");
            
            var predictiveSearch = _search as PredictiveSearch;
            if (predictiveSearch != null)
            {
                Console.WriteLine($@" Elapsed: {_stopwatchRound.ElapsedMilliseconds} ms");
                Console.WriteLine($@" Score calculated {predictiveSearch.ScoreCalculated,10} times");
                Console.WriteLine($@" Score looked up  {predictiveSearch.ScoreLookedUp,10} times");
            }
            
            Console.WriteLine($"Game State:\n{_simulator.GameState}");

            _stopwatchRound.Restart();
        }

        private void Update(int round, int multiplayerHolePosition)
        {
            var result = _search.Search(_simulator.GameState);
            if (result == null) throw new GameOverException();

            if (result.Moves.Any())
            {
                foreach (var move in result.Moves)
                {
                    _simulator.Simulate(move);
                }

                if (Multiplayer && round > 0 && round % 10 == 0)
                {
                    _simulator.GameState.SpawnLines(4, multiplayerHolePosition);
                }
            }
        }

        private void Render()
        {
            //_logger.Info(_simulator.GameState);
            if (PauseTime > 0)
            {
                Thread.Sleep(PauseTime);
            }
        }

        private void LogResults(int rounds, long time)
        {
            Console.WriteLine("Game over");

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Simulator_Results.csv");

            if (!File.Exists(path))
            {
                using (var writer = File.CreateText(path))
                {
                    writer.WriteLine("Pieces;Lines;Score;Level;Time");
                }
            }

            string message = $"{rounds};{_simulator.GameState.Lines};{_simulator.GameState.Score};{_simulator.GameState.Level};{time}\n";
            File.AppendAllText(path, message);
        }
    }
}