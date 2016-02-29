using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.ImageProcessing;
using GameBot.Emulation;
using GameBot.Game.Tetris;
using GameBot.Robot.Actors;
using GameBot.Robot.Rendering;
using SimpleInjector;
using System.Reflection;

namespace GameBot.Robot
{
    public class Launcher
    {
        static void Main(string[] args)
        {
            var container = BuildContainer();

            var engine = container.GetInstance<IEngine>();
            engine.Run();
        }

        static Container BuildContainer()
        {
            var container = new Container();

            container.Register<Emulator>();
            
            container.Register<IEngine, Engine>();

            container.Register<IQuantizer, Quantizer>();
            container.Register<IExecuter, Executer>();

            container.Register<IRenderer, EmguRenderer>();

            var assemblies = new[] { "GameBot.Game.Tetris" };
            foreach (var assemblyName in assemblies)
            {
                var assembly = Assembly.Load(assemblyName);
                container.Register(typeof(IExtractor<>), new[] { assembly });
                container.Register(typeof(IDecider<>), new[] { assembly });

                //container.RegisterCollection(typeof(IAgent), assembly);
                //container.RegisterCollection(typeof(IGameState), assembly);

                // TODO: find dynamic in assembly
                container.Register<IAgent, TetrisAgent>();
                container.Register<IGameState, TetrisGameState>();
            }

            container.Verify();

            return container;
        }
    }
}
