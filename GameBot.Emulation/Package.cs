using SimpleInjector;
using SimpleInjector.Packaging;

namespace GameBot.Emulation
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton(new Emulator());
        }
    }
}
