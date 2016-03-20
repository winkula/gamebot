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

namespace GameBot.Test
{
    [TestFixture]
    public class ExtractorTests
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
            
            var gameState = extractor.Extract(screenshot, new Context<TetrisGameStateFull>());

            Assert.NotNull(gameState);
            Assert.NotNull(gameState.State);
            Assert.NotNull(gameState.State.Board);
            Assert.NotNull(gameState.State.Piece);
            Assert.NotNull(gameState.State.NextPiece);

            Debug.WriteLine(gameState.State.Piece);
            Debug.WriteLine(gameState.State.NextPiece);

            Debug.WriteLine(gameState.State.Board);
        }
    }
}
