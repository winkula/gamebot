using System;
using System.IO;
using GameBot.Game.Tetris.Data;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;

namespace GameBot.Test
{
    [SetUpFixture]
    public class Setup
    {
        [SetUp]
        public void Init()
        {
            ConfigureLogging();
            InitLookups();
        }

        private void ConfigureLogging()
        {
            var config = new LoggingConfiguration();

            var target = new TraceTarget();
            target.Layout = @"${message}";
            config.AddTarget("debugger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "GameBot_ExtractionComparison.csv");
            var fileTarget = new FileTarget();
            fileTarget.Layout = @"${message}";
            fileTarget.FileName = path;
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("ExtractionComparison", LogLevel.Debug, fileTarget));

            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "GameBot_ExtractionDetails.csv");
            fileTarget = new FileTarget();
            fileTarget.Layout = @"${message}";
            fileTarget.FileName = path;
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("ExtractionDetails", LogLevel.Debug, fileTarget));

            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "GameBot_Tests_Fails.txt");
            fileTarget = new FileTarget();
            fileTarget.Layout = @"${message}";
            fileTarget.FileName = path;
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("Fails", LogLevel.Debug, fileTarget));

            LogManager.Configuration = config;
        }

        private void InitLookups()
        {
            var boardLookups = BoardLookups.Instance;
            var tetriminoLookups = TetriminoLookups.Instance;
        }
    }
}