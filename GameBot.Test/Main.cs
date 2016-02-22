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

namespace GameBot.Test
{
    public class Main
    {
        [Test]
        public void Test()
        {
            // download image of display
            // in real: get image as photo of the gameboy screen (input)
            const string url = "https://lifeculturegeekstuff.files.wordpress.com/2011/01/tetris-2.jpg";
            Image image = DownloadImage(url);

            // process image and get display data
            IImageProcessor imageProcessor = new DefaultImageProcessor();
            IDisplayState display = imageProcessor.Process(image);

            // extract game state
            IGameStateExtractor<TetrisGameState> extractor = new TetrisGameStateExtractor();
            TetrisGameState gameState = extractor.Extract(display);

            // decide which commands to press
            IDecider<TetrisGameState> decider = new TetrisDecider();
            ICommand command = decider.Decide(gameState);

            // give commands to command controller (output)
        }

        /// <summary>
        /// Gets the image from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private static Image DownloadImage(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();
            return Image.FromStream(stream);
        }
    }
}
