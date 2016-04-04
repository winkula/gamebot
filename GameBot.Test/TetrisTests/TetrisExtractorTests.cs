using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace GameBot.Test.TetrisTests
{
    [TestFixture]
    public class TetrisExtractorTests
    {
        [Test]
        public void Constructor()
        {
            var extractor = new TetrisExtractor();
        }

        [Test]
        public void Extract()
        {
            var extractor = new TetrisExtractor();
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
