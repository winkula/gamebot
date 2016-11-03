using GameBot.Robot.Ui.Configuration;
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
            // create folders on desktop
            // TODO: remove this (only for debugging)
            string pathDebug = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "debug");
            string pathTest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "test");
            Directory.CreateDirectory(pathDebug);
            Directory.CreateDirectory(pathTest);

            using (var container = new Container())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var config = new ExeConfig();
                var engineMode = config.Read("Robot.Engine.Mode", "Emulated");

                if (engineMode == "Emulated")
                    container.RegisterPackages(GetEmulatedEngineAssembies());
                if (engineMode == "Physical")
                    container.RegisterPackages(GetPhysicalEngineAssembies());
                container.Verify();

                ConfigureLogging();

                Application.Run(container.GetInstance<Window>());
            }
        }

        static IEnumerable<Assembly> GetPhysicalEngineAssembies()
        {
            return GetAssemblies(
                "GameBot.Game.Tetris",
                "GameBot.Engine.Physical",
                "GameBot.Robot.Ui");
        }

        static IEnumerable<Assembly> GetEmulatedEngineAssembies()
        {
            return GetAssemblies(
                "GameBot.Game.Tetris",
                "GameBot.Engine.Emulated",
                "GameBot.Emulation",
                "GameBot.Robot.Ui");
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
            fileTarget.Layout = @"${longdate} | ${level:uppercase=true} | ${pad:padding=-58:inner=${logger}} | ${message}";
            fileTarget.FileName = path;
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));

            LogManager.Configuration = config;
        }
    }
}
