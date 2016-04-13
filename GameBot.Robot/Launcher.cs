using GameBot.Core;
using GameBot.Game.Tetris.Heuristics;

namespace GameBot.Robot
{
    public class Launcher
    {
        static void Main(string[] args)
        {
            // Create dependency injection container
            //var container = Bootstrapper.GetInitializedContainer(Bootstrapper.EngineType.Fast, typeof(TetrisSurviveHeuristic));
            //var container = Bootstrapper.GetInitializedContainer(Bootstrapper.EngineType.Fast, typeof(TetrisHolesHeuristic));
            //var container = Bootstrapper.GetInitializedContainer(Bootstrapper.EngineType.Fast, typeof(TetrisStackingHeuristic));
            var container = Bootstrapper.GetInitializedContainer(Bootstrapper.EngineType.Emulator, typeof(TetrisStackingHeuristic));

            // Run the engine
            var engine = container.GetInstance<IEngine>();
            engine.Run();
        }
    }
}
