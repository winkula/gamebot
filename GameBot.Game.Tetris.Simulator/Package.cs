using GameBot.Core;
using GameBot.Core.Configuration;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace GameBot.Game.Tetris.Simulator
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<IConfig, AppSettingsConfig>();

            container.RegisterSingleton<SimulatorEngine>();
            container.RegisterSingleton<TetrisSimulator>();
        }
    }
}
