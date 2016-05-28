using GameBot.Core;
using GameBot.Core.Ui;
using GameBot.Robot.Ui.Configuration;
using GameBot.Robot.Ui.Debugging;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace GameBot.Robot.Ui
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<IDebugger, Debugger>();
            container.RegisterSingleton<IConfig, ExeConfig>();
            container.RegisterSingleton<IUi, Window>();
        }
    }
}
