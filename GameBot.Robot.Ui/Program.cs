using NLog;
using NLog.Config;
using NLog.Targets;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace GameBot.Robot.Ui
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var container = new Container())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                container.RegisterPackages(GetAssemblies(
                    "GameBot.Game.Tetris",
                    "GameBot.Emulation",
                    "GameBot.Robot",
                    "GameBot.Robot.Ui"));
                container.Verify();

                ConfigureLogging();

                Application.Run(container.GetInstance<Window>());
            }
        }

        static IEnumerable<Assembly> GetAssemblies(params string[] assemblyNames)
        {
            return assemblyNames.Select(x => Assembly.Load(x)).ToList();
        }

        static void ConfigureLogging()
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
