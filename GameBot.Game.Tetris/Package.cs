using GameBot.Core;
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
            container.RegisterSingleton<IExtractor<TetrisGameState>, TetrisExtractor>();
            container.RegisterSingleton<IPlayer<TetrisGameState>, TetrisPlayer>();
            container.RegisterSingleton<ISimulator<TetrisGameState>, TetrisSimulator>();
        }
    }
}
