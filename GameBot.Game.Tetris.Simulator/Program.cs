using System;
using System.IO;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Searching.Heuristics;
using GameBot.Game.Tetris.Simulators;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace GameBot.Game.Tetris.Simulator
{
    class Program
    {
        private const int _games = 100;
        
        static void Main(string[] args)
        {
            ConfigureLogging();
            Simulate();
        }

        static void Simulate()
        {
          var heuristic = new GaHeuristic(-0.317854214844296, - 0.548457056926845 ,- 0.434173510009484, - 0.640044465657585);
            //var heuristic = new YiyuanLeeHeuristic();
            //var heuristic = new ElTetrisHeuristic();
            //var heuristic = new ExperimentalHeuristic();
            //var heuristic = new MaxBergmarkHeuristic();

            var tetrisSearch = new TwoPieceSearch(heuristic);
            //var tetrisSearch = new OnePieceSearch(heuristic);
            //var tetrisSearch = new PredictiveSearch(heuristic);
            //tetrisSearch.Cache = true;
            //var tetrisSearch = new RecursiveSearch(heuristic);
            //tetrisSearch.Depth = 3;

            for (int i = 0; i < _games; i++)
            {
                Console.WriteLine($"Game {i + 1}");

                var tetrisSimulator = new TetrisSimulator();
                var engine = new SimulatorEngine(tetrisSearch, tetrisSimulator);
                engine.FrameUpdateDelay = 1;
                engine.PauseTime = 0;
                engine.Multiplayer = false;
                //engine.MaxHeight = 10;
                engine.Render = false;

                engine.Run();
            }
        }

        static void ConfigureLogging()
        {
            var config = new LoggingConfiguration();

            //var traceTarget = new TraceTarget();
            //traceTarget.Layout = @"${message}";
            //config.AddTarget("debugger", traceTarget);
            //config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, traceTarget));

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "GameBot_Simulator.txt");
            var fileTarget = new FileTarget();
            fileTarget.Layout = @"${message}";
            fileTarget.FileName = path;
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));

            string path2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "GameBot_Time.txt");
            var fileTarget2 = new FileTarget();
            fileTarget2.Layout = @"${message}";
            fileTarget2.FileName = path2;
            config.AddTarget("file2", fileTarget2);
            config.LoggingRules.Add(new LoggingRule("time", LogLevel.Debug, fileTarget2));

            LogManager.Configuration = config;
        }
    }
}
