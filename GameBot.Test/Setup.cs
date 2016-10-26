using System;
using System.IO;
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
        public void ConfigureLogging()
        {
            var config = new LoggingConfiguration();

            var target = new TraceTarget();
            target.Layout = @"${message}";
            config.AddTarget("debugger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "GameBot_Tests_Results.csv");
            var fileTarget = new FileTarget();
            fileTarget.Layout = @"${message}";
            fileTarget.FileName = path;
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("Tests", LogLevel.Debug, fileTarget));

            LogManager.Configuration = config;
        }
    }
}