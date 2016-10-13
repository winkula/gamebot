using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
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
        static void Main()
        {
            using (var container = new Container())
            {
                container.RegisterPackages(GetEmulatedEngineAssembies());
                //container.RegisterPackages(GetPhysicalEngineAssembies());
                container.Verify();

                ConfigureLogging();

                var engine = container.GetInstance<IEngine>();
                engine.Play = true;
                while (true)
                {
                    engine.Step(null);
                }               
            }
        }

        static IEnumerable<Assembly> GetPhysicalEngineAssembies()
        {
            return GetAssemblies(
                "GameBot.Game.Tetris",
                "GameBot.Engine.Physical",
                "GameBot.Robot.Cli");
        }

        static IEnumerable<Assembly> GetEmulatedEngineAssembies()
        {
            return GetAssemblies(
                "GameBot.Game.Tetris",
                "GameBot.Engine.Emulated",
                "GameBot.Emulation",
                "GameBot.Robot.Cli");
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

            var consoleTarget = new ConsoleTarget();
            consoleTarget.Layout = @"${message}";
            config.AddTarget("console", traceTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "GameBot_Log.txt");
            var fileTarget = new FileTarget();
            fileTarget.Layout = @"${longdate}|${level:uppercase=true}|${logger}|${message}";
            fileTarget.FileName = path;
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));

            LogManager.Configuration = config;
        }
    }
}
