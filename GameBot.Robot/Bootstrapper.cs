using GameBot.Core;
using GameBot.Emulation;
using GameBot.Robot.Actuators;
using GameBot.Robot.Cameras;
using GameBot.Robot.Configuration;
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
            Fast,
            Real
        }

        public static Container GetInitializedContainer()
        {
            var config = new Config();
            var engineType = config.Read("Robot.Engine.Mode", EngineType.Real);

            return GetInitializedContainer(engineType);
        }
       
        public static Container GetInitializedContainer(EngineType engineType)
        {
            var container = new Container();

            switch (engineType)
            {
                case EngineType.Fast:
                    container.Register<IEngine, FastEngine>();
                    break;

                case EngineType.Emulator:
                    container.Register<IEngine, EmulatorEngine>();
                    container.RegisterSingleton<Emulator>();
                    container.Register<ICamera, EmulatorCamera>();
                    container.Register<IQuantizer, PassthroughQuantizer>();
                    container.Register<IExecutor, EmulatorExecutor>();
                    container.RegisterSingleton<IActuator, Actuator>();
                    container.RegisterSingleton<ITimeProvider, EmulatorTimeProvider>();
                    break;

                case EngineType.Real:
                    container.Register<IEngine, UiEngine>();
                    container.Register<ICamera, Camera>();
                    container.Register<IQuantizer, Quantizer>();
                    container.Register<IExecutor, Executor>();
                    //container.RegisterSingleton<IActuator, LazyActuator>();
                    container.RegisterSingleton<IActuator, Actuator>();
                    container.RegisterSingleton<ITimeProvider, TimeProvider>();
                    break;

                default:
                    break;
            }

            container.RegisterSingleton<IConfig, Config>(); 
            //container.Register<IRenderer, EmguRenderer>();

            // TODO: remove build-dependency to the "GameBot.Game.Tetris" and load
            // the assembly with "LoadFrom"
            //var assembly = Assembly.LoadFrom(@"C:\Users\Winkler\Documents\visual studio 2015\Projects\GameBot\GameBot.Game.Tetris\bin\x86\Debug\GameBot.Game.Tetris.dll"); //Assembly.Load(assemblyName);

            var assemblyName = "GameBot.Game.Tetris";
            var assembly = Assembly.Load(assemblyName);
            
            container.Register(typeof(IAgent), GetSingleImplementation<IAgent>(assembly));
            container.Register(typeof(IExtractor<>), new[] { assembly });
            container.Register(typeof(IPlayer<>), new[] { assembly });
            container.Register(typeof(ISimulator<>), new[] { assembly });
            
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
