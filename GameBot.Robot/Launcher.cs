using GameBot.Robot.Engines;

namespace GameBot.Robot
{
    public class Launcher
    {
        static bool IsInteractive = false;
        static bool UseEmulator = true;

        static void Main(string[] args)
        {
            // Create dependency injection container
            var container = Bootstrapper.GetInitializedContainer(IsInteractive, UseEmulator);
            
            // Run the engine
            var engine = container.GetInstance<IEngine>();
            engine.Run();
        }
    }
}
