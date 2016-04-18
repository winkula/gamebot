using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Searching;
using GameBot.Emulation;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Heuristics;
using GameBot.Robot.Actuators;
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
        public enum EngineType
        {
            Emulator,
            EmulatorInteractive,
            Fast,
            Real
        }

        public static Container GetInitializedContainer(EngineType type)
        {
            return GetInitializedContainer(type, typeof(TetrisSurviveHeuristic));
        }

        public static Container GetInitializedContainer(EngineType type, Type heuristicType)
        {
            var container = new Container();

            switch (type)
            {
                case EngineType.EmulatorInteractive:
                    container.Register<IEngine, InteractiveEngine>();
                    break;
                case EngineType.Fast:
                    container.Register<IEngine, FastEngine>();
                    break;
                case EngineType.Emulator:
                case EngineType.Real:
                    container.Register<IEngine, Engine>();
                    break;
                default:
                    break;
            }

            if (type == EngineType.EmulatorInteractive || type == EngineType.Emulator)
            {                
                container.RegisterSingleton(new Emulator());

                container.Register<ICamera, EmulatorCamera>();
                container.Register<IQuantizer, PassthroughQuantizer>();
                container.Register<IExecutor, EmulatorExecutor>();
                container.Register<IActuator, Actuator>();

                container.RegisterSingleton<ITimeProvider, EmulatorTimeProvider>();
            }
            else
            {
                container.Register<ICamera, Camera>();
                container.Register<IQuantizer, Quantizer>();
                container.Register<IExecutor, Executor>();
                container.Register<IActuator, Actuator>();

                container.RegisterSingleton<ITimeProvider, TimeProvider>();
            }

            container.Register<IRenderer, EmguRenderer>();

            // TODO: remove build-dependency to the "GameBot.Game.Tetris" and load
            // the assembly with "LoadFrom"
            //var assembly = Assembly.LoadFrom(@"C:\Users\Winkler\Documents\visual studio 2015\Projects\GameBot\GameBot.Game.Tetris\bin\x86\Debug\GameBot.Game.Tetris.dll"); //Assembly.Load(assemblyName);

            var assemblyName = "GameBot.Game.Tetris";
            var assembly = Assembly.Load(assemblyName);
            
            container.Register(typeof(IAgent), GetSingleImplementation<IAgent>(assembly));
            container.Register(typeof(IExtractor<>), new[] { assembly });
            container.Register(typeof(IPlayer<>), new[] { assembly });
            container.Register(typeof(ISimulator<>), new[] { assembly });

            container.Register(typeof(ISearch<>), new[] { assembly });
            //container.Register(typeof(IHeuristic<>), new[] { assembly });
            container.Register(typeof(IHeuristic<>), heuristicType);

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
