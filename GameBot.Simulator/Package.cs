using GameBot.Core;
using GameBot.Robot.Actuators;
using GameBot.Robot.Cameras;
using GameBot.Robot.Configuration;
using GameBot.Robot.Engines;
using GameBot.Robot.Quantizers;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace GameBot.Robot
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<IEngine, FastEngine>();
        }

        private void RegisterFastEngine(Container container)
        {
        }

        private void RegisterEmulatorEngine(Container container)
        {
            container.RegisterSingleton<IEngine, EmulatorEngine>();
            container.RegisterSingleton<ICamera, EmulatorCamera>();
            container.RegisterSingleton<IQuantizer, PassthroughQuantizer>();
            container.RegisterSingleton<ITimeProvider, EmulatorTimeProvider>();
            container.RegisterSingleton<IActuator, LazyActuator>();
        }

        private void RegisterRealEngine(Container container)
        {
            container.RegisterSingleton<IEngine, UiEngine>();
            container.RegisterSingleton<ICamera, Camera>();
            container.RegisterSingleton<IQuantizer, Quantizer>();
            container.RegisterSingleton<IActuator, Actuator>();
            //container.RegisterSingleton<IActuator, LazyActuator>();
            container.RegisterSingleton<ITimeProvider, TimeProvider>();
        }
    }
}
