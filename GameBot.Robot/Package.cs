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
            var config = new Config();
            var mode = config.Read<EngineMode>("Robot.Engine.Mode");

            switch (mode)
            {
                case EngineMode.Emulator:
                    RegisterEmulatorEngine(container);
                    break;
                case EngineMode.Fast:
                    RegisterFastEngine(container);
                    break;
                case EngineMode.Real:
                    RegisterRealEngine(container);
                    break;
                default:
                    break;
            }           
        }

        private void RegisterFastEngine(Container container)
        {
            container.RegisterSingleton<IEngine, FastEngine>();
        }

        private void RegisterEmulatorEngine(Container container)
        {
            container.RegisterSingleton<IEngine, EmulatorEngine>();
            container.RegisterSingleton<ICamera, EmulatorCamera>();
            container.RegisterSingleton<IQuantizer, PassthroughQuantizer>();
            container.RegisterSingleton<IExecutor, EmulatorExecutor>();
            container.RegisterSingleton<IActuator, LazyActuator>();
            container.RegisterSingleton<ITimeProvider, EmulatorTimeProvider>();
        }

        private void RegisterRealEngine(Container container)
        {
            container.RegisterSingleton<IEngine, UiEngine>();
            container.RegisterSingleton<ICamera, Camera>();
            container.RegisterSingleton<IQuantizer, Quantizer>();
            container.RegisterSingleton<IExecutor, Executor>();
            container.RegisterSingleton<IActuator, Actuator>();
            container.RegisterSingleton<ITimeProvider, TimeProvider>();
        }
    }
}
