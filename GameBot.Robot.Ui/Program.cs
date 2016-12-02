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
            GameBot();
        }
        
        static void GameBot()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var config = new ExeConfig();
            var engineMode = config.Read("Robot.Engine.Mode", "Emulated");
            var logLevelString = config.Read("Robot.Ui.LogLevel", "Error");

            using (var container = new Container())
            {
                // register packages
                if (engineMode == "Emulated") container.RegisterPackages(GetEmulatedEngineAssembies());
                if (engineMode == "Physical") container.RegisterPackages(GetPhysicalEngineAssembies());

                // verify packages
                container.Verify();

                // config logging framework
                ConfigureLogging(logLevelString);
                var logger = LogManager.GetCurrentClassLogger();

                try
                {
                    Application.Run(container.GetInstance<Window>());
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex);
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

        static void ConfigureLogging(string logLevelString)
        {
            var logLevel = GetLogLevel(logLevelString);
            var config = new LoggingConfiguration();

#if DEBUG
            var traceTarget = new TraceTarget
            {
                Layout = @"${message}"
            };
            config.AddTarget("debugger", traceTarget);
            config.LoggingRules.Add(new LoggingRule("*", logLevel, traceTarget));
#endif
            var fileTarget = new FileTarget
            {
                Layout = @"${longdate} | ${level:uppercase=true} | ${pad:padding=-58:inner=${logger}} | ${message}",
                FileName = GetLogPath()
            };
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", logLevel, fileTarget));
            
            // TODO: remove this after measurement
            var fileTargetPieceLogging = new FileTarget
            {
                Layout = @"${message}",
                FileName = GetLogPath("Pieces.txt")
            };
            config.AddTarget("file", fileTargetPieceLogging);
            config.LoggingRules.Add(new LoggingRule("PieceLogger", LogLevel.Info, fileTargetPieceLogging));
            
            LogManager.Configuration = config;
        }

        static string GetLogPath(string filename = "GameBot_Log.txt")
        {
#if DEBUG
            // on desktop in debug mode
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), filename);
#endif
            // in current folder in release mode
            return filename;
        }

        static LogLevel GetLogLevel(string logLevelString)
        {
            try
            {
                return LogLevel.FromString(logLevelString);
            }
            catch (ArgumentException)
            {
                // ignore
                return LogLevel.Error;
            }
        }
    }
}
