using NUnit.Framework;
using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GameBot.Core.ImageProcessing;
using GameBot.Game.Tetris;
using GameBot.Core.Extractors;
using SimpleInjector;
using GameBot.Robot;
using System.Reflection;

namespace GameBot.Test
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void DependencyInjection()
        {
            // set up dependency injection container
            var container = new Container();

            // 2. Configure the container (register)
            container.Register<IImageProcessor, DefaultImageProcessor>();
            container.Register<ICommandController, DefaultCommandController>();

            //container.Register<IGameStateExtractor<TetrisGameState>, TetrisGameStateExtractor>();
            //container.Register<IDecider<TetrisGameState>, TetrisDecider>();

            var assemblies = new[] { "GameBot.Game.Tetris" };
            foreach (var assemblyName in assemblies)
            {
                var assembly = Assembly.Load(assemblyName);
                container.Register(typeof(IGameStateExtractor<>), new[] { assembly });
                container.Register(typeof(IDecider<>), new[] { assembly });
            }
            
            // 3. Optionally verify the container's configuration.
            container.Verify();
        }

        public void Process(Container container)
        {
            // download image of display
            // in real: get image as photo of the gameboy screen (input)
            const string url = "https://lifeculturegeekstuff.files.wordpress.com/2011/01/tetris-2.jpg";
            Image image = DownloadImage(url);

            // process image and get display data
            IImageProcessor imageProcessor = container.GetInstance<IImageProcessor>();
            IDisplayState display = imageProcessor.Process(image);

            // extract game state
            IGameStateExtractor<TetrisGameState> extractor = container.GetInstance<IGameStateExtractor<TetrisGameState>>();
            TetrisGameState gameState = extractor.Extract(display);

            // decide which commands to press
            IDecider<TetrisGameState> decider = container.GetInstance<IDecider<TetrisGameState>>();
            ICommand command = decider.Decide(gameState);

            // give commands to command controller (output)
            ICommandController commandController = container.GetInstance<ICommandController>();
            commandController.Execute(command);
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
