using GameBot.Core;
using GameBot.Robot.Actuators;
using GameBot.Robot.Cameras;
using GameBot.Robot.Configuration;
using GameBot.Robot.Engines;
using GameBot.Robot.Executors;
using GameBot.Robot.Quantizers;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace GameBot.Robot
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            //container.RegisterSingleton<IConfig, Config>();

            /*
            // fast
            container.Register<IEngine, FastEngine>();
            */

            /*
            // emulator
            container.Register<IEngine, EmulatorEngine>();
            container.Register<ICamera, EmulatorCamera>();
            container.Register<IQuantizer, PassthroughQuantizer>();
            container.Register<IExecutor, EmulatorExecutor>();
            //container.RegisterSingleton<IActuator, Actuator>();
            container.RegisterSingleton<ITimeProvider, EmulatorTimeProvider>();
            */
            
            // real
            container.RegisterSingleton<IEngine, UiEngine>();
            container.RegisterSingleton<ICamera, Camera>();
            container.RegisterSingleton<IQuantizer, Quantizer>();
            container.RegisterSingleton<IExecutor, Executor>();
            container.RegisterSingleton<IActuator, LazyActuator>();
            //container.RegisterSingleton<IActuator, Actuator>();
            container.RegisterSingleton<ITimeProvider, TimeProvider>();
        }
    }
}
