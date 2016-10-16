using System;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class PieceExtractorTests
    {
        private PieceExtractor _pieceExtractor;

        [SetUp]
        public void Init()
        {
            _pieceExtractor = new PieceExtractor();
        }
        
        [Test]
        public void ExtractNextPiece()
        {
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", TimeSpan.Zero);

            var nextPiece = _pieceExtractor.ExtractNextPiece(screenshot);

            Assert.AreEqual(Tetromino.L, nextPiece);
        }

        [Test]
        public void ExtractNextPieceInvalid()
        {
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_2.png", TimeSpan.Zero);

            Assert.Throws<ApplicationException>(() =>
            {
                _pieceExtractor.ExtractNextPiece(screenshot);
            });
        }
    }
}
