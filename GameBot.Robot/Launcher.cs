using GameBot.Core;

namespace GameBot.Robot
{
    public class Launcher
    {
        static void Main(string[] args)
        {
            //RunSimulations();
            RunMain();
        }

        static void RunMain()
        {
            using (var container = Bootstrapper.GetInitializedContainer())
            {
                container.GetInstance<IEngine>().Run();
            }
        }

        static void RunSimulations()
        {
            for (int i = 0; i < 20; i++)
            {
                var container = Bootstrapper.GetInitializedContainer(Bootstrapper.EngineType.Fast);
                container.GetInstance<IEngine>().Run();
            }
        }
    }
}
