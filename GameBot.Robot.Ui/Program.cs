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
#if DEBUG
            CreateFolders();
#endif

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
                var logger = LogManager.GetCurrentClassLogger();
                try
                {
                    Application.Run(container.GetInstance<Window>());
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
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
            return assemblyNames.Select(Assembly.Load).ToList();
        }

        static void CreateFolders()
        {
            // create folders on desktop
            string pathDebug = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "debug");
            string pathTest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "test");
            Directory.CreateDirectory(pathDebug);
            Directory.CreateDirectory(pathTest);
        }

        static void ConfigureLogging()
        {
            var config = new LoggingConfiguration();

#if DEBUG
            var traceTarget = new TraceTarget
            {
                Layout = @"${message}"
            };
            config.AddTarget("debugger", traceTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, traceTarget));
#endif
            var fileTarget = new FileTarget
            {
                Layout = @"${longdate} | ${level:uppercase=true} | ${pad:padding=-58:inner=${logger}} | ${message}",
                FileName = GetLogPath()
            };
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", GetLogLevel(), fileTarget));

            LogManager.Configuration = config;
        }

        static string GetLogPath()
        {
            const string filename = "GameBot_Log.txt";
#if DEBUG
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), filename);
#endif
            return filename;
        }

        static LogLevel GetLogLevel()
        {
#if DEBUG
            return LogLevel.Debug;
#endif
            return LogLevel.Error;
        }
    }
}
