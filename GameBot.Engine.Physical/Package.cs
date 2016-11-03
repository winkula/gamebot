using GameBot.Core;
using GameBot.Core.Executors;
using GameBot.Core.Quantizers;
using GameBot.Engine.Physical.Actuators;
using GameBot.Engine.Physical.Cameras;
using GameBot.Engine.Physical.Clocks;
using GameBot.Engine.Physical.Quantizers;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace GameBot.Engine.Physical
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<IEngine, PhysicalEngine>();

            //container.RegisterSingleton<IActuator, PhysicalActuator>();
            container.RegisterSingleton<IActuator, LazyActuator>();
            container.RegisterSingleton<ICamera, PhysicalCamera>();
            container.RegisterSingleton<IClock, PhysicalClock>();
            container.RegisterSingleton<IExecutor, Executor>();

            //container.RegisterSingleton<IQuantizer, Quantizer>();
            container.RegisterSingleton<IQuantizer, MorphologyQuantizer>();
        }
    }
}
