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
        private PieceMatcher _pieceMatcher;
        private PieceExtractor _pieceExtractor;

        [SetUp]
        public void Init()
        {
            _pieceMatcher = new PieceMatcher();
            _pieceExtractor = new PieceExtractor(_pieceMatcher);
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

        [Test]
        public void ExtractNextPieceFuzzy()
        {
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", TimeSpan.Zero);

            var tetromino = _pieceExtractor.ExtractNextPieceFuzzy(screenshot, 0.5);

            Assert.NotNull(tetromino);
            Assert.AreEqual(Tetromino.L, tetromino);
        }
    }
}
