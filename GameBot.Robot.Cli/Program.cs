using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using GameBot.Robot.Engines;
using NLog;
using NLog.Config;
using NLog.Targets;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Robot.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            // CLI = command line interface
            // for tests and measurements

            Simulate();
        }
        
        static void Simulate()
        {
            using (var container = new Container())
            {
                container.RegisterSingleton<IEngine, FastEngine>();

                container.RegisterPackages(GetAssemblies(
                    "GameBot.Game.Tetris",
                    "GameBot.Emulation",
                    "GameBot.Robot"));
                container.Verify();

                ConfigureLogging();

                var engine = container.GetInstance<IEngine>();
                engine.Run();
            }
        }

        static IEnumerable<Assembly> GetAssemblies(params string[] assemblyNames)
        {
            return assemblyNames.Select(x => Assembly.Load(x)).ToList();
        }

        static void ConfigureLogging()
        {
            var config = new LoggingConfiguration();

            var traceTarget = new TraceTarget();
            traceTarget.Layout = @"${message}";
            config.AddTarget("debugger", traceTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, traceTarget));

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "GameBot_Simulator.txt");
            var fileTarget = new FileTarget();
            fileTarget.Layout = @"${longdate}|${level:uppercase=true}|${logger}|${message}";
            fileTarget.FileName = path;
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));

            LogManager.Configuration = config;
        }
    }
}
