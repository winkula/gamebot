using GameBot.Core.Data;
using GameBot.Game.Tetris;
using GameBot.Robot.Configuration;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Drawing;

namespace GameBot.Test.TetrisTests
{
    [TestFixture]
    public class TetrisExtractorTests
    {
        [Test]
        public void Constructor()
        {
            var config = new Config();

            var extractor = new TetrisExtractor(config);
        }

        [Test]
        public void Extract()
        {
            var config = new Config();

            var extractor = new TetrisExtractor(config);
            var image = Image.FromFile("Screenshots/tetris_play_1.png");
            var screenshot = new Screenshot(image, TimeSpan.Zero);
            
            var gameState = extractor.Extract(screenshot);

            Assert.NotNull(gameState);
            Assert.NotNull(gameState.Board);
            Assert.NotNull(gameState.Piece);
            Assert.NotNull(gameState.NextPiece);

            Debug.WriteLine(gameState.Piece);
            Debug.WriteLine(gameState.NextPiece);

            Debug.WriteLine(gameState.Board);
        }
    }
}
