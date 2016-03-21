using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Emulation;
using GameBot.Robot.Cameras;
using GameBot.Robot.Engines;
using GameBot.Robot.Executors;
using GameBot.Robot.Quantizers;
using GameBot.Robot.Renderers;
using SimpleInjector;
using System;
using System.Linq;
using System.Reflection;

namespace GameBot.Robot
{
    public class Bootstrapper
    {
        public static Container GetInitializedContainer()
        {
            return GetInitializedContainer(false, false);
        }

        public static Container GetInitializedContainer(bool interactive, bool emulator)
        {
            var container = new Container();

            if (interactive)
            {
                container.Register<IEngine, InteractiveEngine>();
            }
            else
            {
                container.Register<IEngine, Engine>();
                //container.Register<IEngine, FastEngine>();
            }

            if (emulator)
            {
                container.RegisterSingleton(new Emulator());

                container.Register<ICamera, EmulatorCamera>();
                container.Register<IQuantizer, PassthroughQuantizer>();
                container.Register<IExecutor, EmulatorExecutor>();
            }
            else
            {
                container.Register<ICamera, Camera>();
                container.Register<IQuantizer, Quantizer>();
                container.Register<IExecutor, Executor>();
            }

            container.Register<IRenderer, EmguRenderer>();

            // TODO: remove build-dependency to the "GameBot.Game.Tetris" and load
            // the assembly with "LoadFrom"
            //var assembly = Assembly.LoadFrom(@"C:\Users\Winkler\Documents\visual studio 2015\Projects\GameBot\GameBot.Game.Tetris\bin\x86\Debug\GameBot.Game.Tetris.dll"); //Assembly.Load(assemblyName);

            var assemblyName = "GameBot.Game.Tetris";
            var assembly = Assembly.Load(assemblyName);

            container.Register(typeof(IGameState), GetSingleImplementation<IGameState>(assembly));
            container.Register(typeof(IAgent), GetSingleImplementation<IAgent>(assembly));
            container.Register(typeof(IExtractor<>), new[] { assembly });
            container.Register(typeof(IDecider<>), new[] { assembly });

            container.Verify();

            return container;
        }

        private static Type GetSingleImplementation<T>(Assembly assembly)
        {
            return assembly
                .GetExportedTypes()
                .Where(x => x.GetInterfaces().Any(y => y == typeof(T)))
                .Single();
        }
    }
}
