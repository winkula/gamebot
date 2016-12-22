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
        private const int _games = 50;
        
        static void Main(string[] args)
        {
            ConfigureLogging();
            Simulate();
        }

        static void Simulate()
        {
            //var heuristic = new GaHeuristic(-0.10974, -0.02329, -0.48643, -0.21246);
            var heuristic = new YiyuanLeeHeuristic();
            //var heuristic = new ExperimentalHeuristic();
            //var heuristic = new MaxBergmarkHeuristic();

            var tetrisSearch = new SimpleSearch(heuristic);
            //var tetrisSearch = new PredictiveSearch(heuristic);
            //tetrisSearch.Cache = true;
            //var tetrisSearch = new RecursiveSearch(heuristic);
            //tetrisSearch.Depth = 3;

            for (int i = 0; i < _games; i++)
            {
                var tetrisSimulator = new TetrisSimulator();
                var engine = new SimulatorEngine(tetrisSearch, tetrisSimulator);
                engine.FrameUpdateDelay = 1;
                engine.PauseTime = 100;
                engine.Multiplayer = true;

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
