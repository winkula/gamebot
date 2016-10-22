using GameBot.Core;
using GameBot.Core.Executors;
using GameBot.Engine.Emulated.Actuators;
using GameBot.Engine.Emulated.Cameras;
using GameBot.Engine.Emulated.Clocks;
using GameBot.Engine.Emulated.Quantizers;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace GameBot.Engine.Emulated
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<IEngine, EmulatorEngine>();

            container.RegisterSingleton<IActuator, EmulatedActuator>();
            container.RegisterSingleton<ICamera, EmulatedCamera>();
            container.RegisterSingleton<IClock, EmulatedClock>();
            container.RegisterSingleton<IExecutor, Executor>();
            container.RegisterSingleton<IQuantizer, PassthroughQuantizer>();
        }        
    }
}
