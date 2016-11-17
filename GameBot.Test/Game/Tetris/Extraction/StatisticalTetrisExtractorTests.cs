using System.Text;
using GameBot.Core.Configuration;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using NLog;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class StatisticalTetrisExtractorTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private int _nextPiece;
        private int _nextPieceRecognized;
        private int _unknownSpawnedPiece;
        private int _unknownSpawnedPieceRecognized;

        private TetrisExtractor _extractor;

        [TestFixtureSetUp]
        public void Init()
        {
            var config = TestHelper.GetFakeConfig().Object;
            _extractor = new TetrisExtractor(config);
        }

        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesNextPiecePositives))]
        public void RecognizeNextPiece(string imageKey, IScreenshot screenshot, Tetrimino nextPieceExpected)
        {
            _nextPiece++;

            var tetromino = _extractor.ExtractNextPiece(screenshot);

            Assert.NotNull(tetromino);
            Assert.AreEqual(nextPieceExpected, tetromino);

            _nextPieceRecognized++;
        }

        // not relevant because not an issue in A-Type mode
        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesNextPieceNegativesNull))]
        public void NotRecognizeNextPiece(string imageKey, IScreenshot screenshot)
        {
            var tetromino = _extractor.ExtractNextPiece(screenshot);

            Assert.Null(tetromino);
        }

        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesSpawnedPiecePositives))]
        public void RecognizeUnknownSpawnedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            _unknownSpawnedPiece++;

            var searchHeight = currentPieceExpected.FallHeight;
            var piece = _extractor.ExtractSpawnedPiece(screenshot, searchHeight);

            Assert.NotNull(piece);
            Assert.AreEqual(currentPieceExpected, piece);

            _unknownSpawnedPieceRecognized++;
        }
        
        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesSpawnedPieceNegativesNull))]
        public void NotRecognizeUnknownSpawnedPiece(string imageKey, IScreenshot screenshot)
        {
            _unknownSpawnedPiece++;

            // TODO: make tests with higher search distance!
            var searchHeight = 3;
            var piece = _extractor.ExtractSpawnedPiece(screenshot, searchHeight);

            Assert.Null(piece);

            _unknownSpawnedPieceRecognized++;
        }
        
        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesSpawnedPiecePositives))]
        public void RecognizeKnownSpawnedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            // TODO: implement?
        }

        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesTouchedPieces))]
        public void NotRecognizeKnownSpawnedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            // TODO: implement?
        }

        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesMovedPiece))]
        public void RecognizeMovedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected, Move move)
        {
            // TODO: implement?
        }

        [TestFixtureTearDown]
        public void Summary()
        {
            _logger.Info(BuildSummaryString("Test recognize unknown next piece", _nextPiece, _nextPieceRecognized));
            _logger.Info(BuildSummaryString("Test recognize unknown spawned piece", _unknownSpawnedPiece, _unknownSpawnedPieceRecognized));
        }

        private string BuildSummaryString(string title, int total, int recognized)
        {
            var sb = new StringBuilder();

            sb.AppendLine(title);
            sb.AppendLine($"{recognized} / {total}  ({(double)recognized / total * 100.0:F})");

            return sb.ToString();
        }
    }
}
