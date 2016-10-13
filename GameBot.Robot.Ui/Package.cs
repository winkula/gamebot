using GameBot.Core;
using GameBot.Robot.Ui.Configuration;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace GameBot.Robot.Ui
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<IConfig, ExeConfig>();
            container.RegisterSingleton<Window>();
        }
    }
}
