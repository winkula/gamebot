using NUnit.Framework;
using GameBot.Core;
using System.Drawing;
using System.IO;
using System.Net;
using GameBot.Game.Tetris;
using SimpleInjector;
using System.Reflection;
using GameBot.Core.Data;
using GameBot.Robot.Actors;
using System.Linq;
using GameBot.Robot.Sensors;

namespace GameBot.Test
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void DependencyInjection()
        {
            // 1. Set up dependency injection container
            var container = new Container();

            // 2. Configure the container (register)
            container.Register<IQuantizer, Quantizer>();
            container.Register<IExecuter, Executer>();

            var assemblies = new[] { "GameBot.Game.Tetris" };
            foreach (var assemblyName in assemblies)
            {
                var assembly = Assembly.Load(assemblyName);
                container.Register(typeof(IExtractor<>), new[] { assembly });
                container.Register(typeof(IDecider<>), new[] { assembly });
                container.RegisterCollection(typeof(IAgent), assembly);
                //container.RegisterCollection(typeof(IGameState), assembly);
            }

            // 3. Optionally verify the container's configuration.
            container.Verify();

            Assert.NotNull(container.GetInstance<IQuantizer>());
            Assert.NotNull(container.GetInstance<IExecuter>());
            Assert.NotNull(container.GetInstance<IExtractor<TetrisGameState>>());
            Assert.NotNull(container.GetInstance<IDecider<TetrisGameState>>());
            Assert.NotNull(container.GetAllInstances<IAgent>());
            //Assert.NotNull(container.GetAllInstances<IGameState>());
            Assert.AreEqual(1, container.GetAllInstances<IAgent>().ToList().Count);
            //Assert.AreEqual(1, container.GetAllInstances<IGameState>().ToList().Count);
        }

        public void Process(Container container)
        {
            // download image of display
            // in real: get image as photo of the gameboy screen (input)
            const string url = "https://lifeculturegeekstuff.files.wordpress.com/2011/01/tetris-2.jpg";
            Image image = DownloadImage(url);

            // process image and get display data
            IQuantizer quantizer = container.GetInstance<IQuantizer>();
            IScreenshot screenshot = quantizer.Quantize(image);

            // extract game state
            IContext<TetrisGameState> context = new Context<TetrisGameState>();
            IExtractor<TetrisGameState> extractor = container.GetInstance<IExtractor<TetrisGameState>>();
            TetrisGameState gameState = extractor.Extract(screenshot, context);

            // decide which commands to press
            IDecider<TetrisGameState> decider = container.GetInstance<IDecider<TetrisGameState>>();
            ICommands commands = decider.Decide(gameState, context);

            // give commands to command controller (output)
            IExecuter commandController = container.GetInstance<IExecuter>();
            commandController.Execute(commands);
        }

        /// <summary>
        /// Gets the image from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private static Image DownloadImage(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();
            return Image.FromStream(stream);
        }
    }
}
