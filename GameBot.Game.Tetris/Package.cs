using GameBot.Core;
using GameBot.Game.Tetris.Agents;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Searching.Heuristics;
using SimpleInjector;
using SimpleInjector.Packaging;
using System.Reflection;

namespace GameBot.Game.Tetris
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            /*
            container.Register(typeof(IAgent), Assembly.GetExecutingAssembly);
            container.RegisterCollection(typeof(IExtractor<>), new[] { });
            container.Register(typeof(IPlayer<>), Assembly.GetExecutingAssembly);
            container.Register(typeof(ISimulator<>), Assembly.GetExecutingAssembly);*/
           
            container.RegisterSingleton<IAgent, TetrisAgent>();
            //container.RegisterSingleton<IExtractor<TetrisGameState>, TetrisExtractor>();
            container.RegisterSingleton<TetrisExtractor>();
            container.RegisterSingleton<TetrisAi>();
            container.RegisterSingleton<ISearch, SimpleSearch>();
            container.RegisterSingleton<IHeuristic, YiyuanLeeHeuristic>();
            container.RegisterSingleton<ISimulator<GameState>, TetrisSimulator>();
        }
    }
}
