using GameBot.Core;
using GameBot.Game.Tetris.Heuristics;

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
            // Create dependency injection container
            //var container = Bootstrapper.GetInitializedContainer(Bootstrapper.EngineType.Fast, typeof(TetrisSurviveHeuristic));
            //var container = Bootstrapper.GetInitializedContainer(Bootstrapper.EngineType.Fast, typeof(TetrisHolesHeuristic));
            //var container = Bootstrapper.GetInitializedContainer(Bootstrapper.EngineType.Fast, typeof(TetrisStackingHeuristic));
            var container = Bootstrapper.GetInitializedContainer(Bootstrapper.EngineType.Emulator, typeof(TetrisSurviveHeuristic));
            //var container = Bootstrapper.GetInitializedContainer(Bootstrapper.EngineType.EmulatorInteractive, typeof(TetrisSurviveHeuristic));

            // Run the engine
            var engine = container.GetInstance<IEngine>();
            engine.Run();
        }

        static void RunSimulations()
        {
            for (int i = 0; i < 20; i++)
            {
                //var container = Bootstrapper.GetInitializedContainer(Bootstrapper.EngineType.Fast, typeof(TetrisSurviveHeuristic));
                var container = Bootstrapper.GetInitializedContainer(Bootstrapper.EngineType.Fast, typeof(TetrisHolesHeuristic));
                container.GetInstance<IEngine>().Run();
            }
        }
    }
}
