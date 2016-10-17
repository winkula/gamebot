using Emgu.CV;
using GameBot.Core.Configuration;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using NLog;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class RealTetrisExtractorTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private TetrisExtractor _extractor;

        [TestFixtureSetUp]
        public void Init()
        {
            var config = new AppSettingsConfig();
            _extractor = new TetrisExtractor(config);
        }
        
        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesSpawnedPiece))]
        public void RecognizeSpawnedPieceOrigin(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            if (currentPieceExpected.IsFallen)
            {
                Assert.Ignore("Piece is not in the spawn origin");
            }

            var piece = _extractor.ExtractSpawnedPieceOrigin(screenshot);
            
            Assert.NotNull(piece);
            Assert.AreEqual(currentPieceExpected, piece);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesSpawnedPieceNull))]
        public void NotRecognizeSpawnedPieceOrigin(string imageKey, IScreenshot screenshot)
        {
            var piece = _extractor.ExtractSpawnedPieceOrigin(screenshot);

            Assert.Null(piece);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesSpawnedPiece))]
        public void RecognizeSpawnedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            var searchHeight = currentPieceExpected.FallHeight;
            var piece = _extractor.ExtractSpawnedPiece(screenshot, searchHeight);

            Assert.NotNull(piece);
            Assert.AreEqual(currentPieceExpected, piece);
        }
        
        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesSpawnedPieceNull))]
        public void NotRecognizeSpawnedPiece(string imageKey, IScreenshot screenshot)
        {
            var searchHeight = 3;
            var piece = _extractor.ExtractSpawnedPiece(screenshot, searchHeight);
            
            Assert.Null(piece);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesNextPiece))]
        public void RecognizeNextPiece(string imageKey, IScreenshot screenshot, Tetromino nextPieceExpected)
        {
            var tetromino = _extractor.ExtractNextPiece(screenshot);

            Assert.NotNull(tetromino);
            Assert.AreEqual(nextPieceExpected, tetromino);
        }
        
        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesNextPieceNull))]
        public void NotRecognizeNextPiece(string imageKey, IScreenshot screenshot)
        {
            var tetromino = _extractor.ExtractNextPiece(screenshot);

            Assert.Null(tetromino);
        }
    }
}
