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
        private const double ProbabilityThreshold = 0.8;

        private PieceMatcher _pieceMatcher;
        private PieceExtractor _pieceExtractor;

        [SetUp]
        public void Init()
        {
            _pieceMatcher = new PieceMatcher();
            _pieceExtractor = new PieceExtractor(_pieceMatcher);
        }

        [Test]
        public void ExtractPieceSpawnedFuzzy1()
        {
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", TimeSpan.Zero);

            var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, 3, 0.5);

            Assert.NotNull(result.Item1);
            Assert.AreEqual(Tetromino.S, result.Item1.Tetromino);
        }

        [Test]
        public void ExtractPieceSpawnedFuzzy2()
        {

            var screenshot = new EmguScreenshot("Screenshots/tetris_play_2.png", TimeSpan.Zero);

            var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, 5, ProbabilityThreshold);

            Assert.Null(result.Item1);

            result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, 6, ProbabilityThreshold);

            Assert.NotNull(result.Item1);
            Assert.AreEqual(Tetromino.Z, result.Item1.Tetromino);
        }

        [Test]
        public void ExtractPieceFuzzy()
        {
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_2.png", TimeSpan.Zero);

            var result = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, new Piece(Tetromino.Z, 0, 0, -6), 0, ProbabilityThreshold);
            
            Assert.NotNull(result.Item1);
            Assert.AreEqual(Tetromino.Z, result.Item1.Tetromino);

            result = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, new Piece(Tetromino.Z, 0, 0, -4), 0, ProbabilityThreshold);

            Assert.Null(result.Item1);

            result = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, new Piece(Tetromino.Z, 0, 0, -4), 2, ProbabilityThreshold);

            Assert.NotNull(result.Item1);
            Assert.AreEqual(Tetromino.Z, result.Item1.Tetromino);
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
