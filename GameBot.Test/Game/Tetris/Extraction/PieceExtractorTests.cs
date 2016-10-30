using System;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Extraction.Matchers;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class PieceExtractorTests
    {
        private const double _probabilityThreshold = 0.8;

        private TemplateMatcher _templateMatcher;
        private PieceExtractor _pieceExtractor;

        [SetUp]
        public void Init()
        {
            _templateMatcher = new TemplateMatcher();
            _pieceExtractor = new PieceExtractor(_templateMatcher);
        }

        [Test]
        public void ExtractPieceSpawnedFuzzy1()
        {
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", TimeSpan.Zero);

            var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, 3);

            Assert.True(result.IsAccepted(0.5));
            Assert.AreEqual(Tetrimino.S, result.Result.Tetrimino);
        }

        [Test]
        public void ExtractPieceSpawnedFuzzy2()
        {
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_2.png", TimeSpan.Zero);

            var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, 5);

            Assert.True(result.IsRejected(_probabilityThreshold));
            Assert.False(result.IsAccepted(_probabilityThreshold));

            result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, 6);

            Assert.True(result.IsAccepted(_probabilityThreshold));
            Assert.AreEqual(Tetrimino.Z, result.Result.Tetrimino);
        }

        [Test]
        public void ExtractPieceFuzzy()
        {
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_2.png", TimeSpan.Zero);

            var result = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, new Piece(Tetrimino.Z, 0, 0, -6), 0);

            Assert.True(result.IsAccepted(_probabilityThreshold));
            Assert.False(result.IsRejected(_probabilityThreshold));
            Assert.AreEqual(Tetrimino.Z, result.Result.Tetrimino);

            result = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, new Piece(Tetrimino.Z, 0, 0, -4), 0);

            Assert.True(result.IsRejected(_probabilityThreshold));
            Assert.False(result.IsAccepted(_probabilityThreshold));

            result = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, new Piece(Tetrimino.Z, 0, 0, -4), 2);

            Assert.True(result.IsAccepted(_probabilityThreshold));
            Assert.False(result.IsRejected(_probabilityThreshold));
            Assert.AreEqual(Tetrimino.Z, result.Result.Tetrimino);
        }

        [Test]
        public void ExtractNextPiece()
        {
            var screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", TimeSpan.Zero);

            var nextPiece = _pieceExtractor.ExtractNextPiece(screenshot);

            Assert.AreEqual(Tetrimino.L, nextPiece);
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

            var result = _pieceExtractor.ExtractNextPieceFuzzy(screenshot);

            Assert.True(result.IsAccepted(0.5));
            Assert.False(result.IsRejected(0.5));
            Assert.AreEqual(Tetrimino.L, result.Result);
        }
        
        [TestCase("Screenshots/fail00.png", Tetrimino.J)]
        public void ExtractNextPieceFuzzyFailCandidates(string path, Tetrimino expected)
        {
            var screenshot = new EmguScreenshot(path, TimeSpan.Zero);

            var result = _pieceExtractor.ExtractNextPieceFuzzy(screenshot);
            
            Assert.AreEqual(expected, result.Result);
        }
    }
}
