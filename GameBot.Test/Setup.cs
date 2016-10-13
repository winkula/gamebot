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

            LogManager.Configuration = config;
        }
    }
}