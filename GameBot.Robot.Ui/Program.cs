using NLog;
using NLog.Config;
using NLog.Targets;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
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

            var traceTarget = new TraceTarget();
            traceTarget.Layout = @"${message}";
            config.AddTarget("debugger", traceTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, traceTarget));

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
