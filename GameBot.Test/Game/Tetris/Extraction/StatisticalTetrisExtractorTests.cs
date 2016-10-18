using System.Text;
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
    public class StatisticalTetrisExtractorTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private int _nextPiece;
        private int _nextPieceRecognized;
        private int _unknownSpawnedPiece;
        private int _unknownSpawnedPieceRecognized;
        private int _knownSpawnedPiece;
        private int _knownSpawnedPieceRecognized;
        private int _movedPiece;
        private int _movedPieceRecognized;

        private TetrisExtractor _extractor;

        [TestFixtureSetUp]
        public void Init()
        {
            var config = new AppSettingsConfig();
            _extractor = new TetrisExtractor(config);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesNextPiecePositives))]
        public void RecognizeNextPiece(string imageKey, IScreenshot screenshot, Tetromino nextPieceExpected)
        {
            _nextPiece++;

            var tetromino = _extractor.ExtractNextPiece(screenshot);

            Assert.NotNull(tetromino);
            Assert.AreEqual(nextPieceExpected, tetromino);

            _nextPieceRecognized++;
        }

        // not relevant because not an issue in A-Type mode
        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesNextPieceNegativesNull))]
        public void NotRecognizeNextPiece(string imageKey, IScreenshot screenshot)
        {
            var tetromino = _extractor.ExtractNextPiece(screenshot);

            Assert.Null(tetromino);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesSpawnedPiecePositives))]
        public void RecognizeUnknownSpawnedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            _unknownSpawnedPiece++;

            var searchHeight = currentPieceExpected.FallHeight;
            var piece = _extractor.ExtractSpawnedPiece(screenshot, searchHeight);

            Assert.NotNull(piece);
            Assert.AreEqual(currentPieceExpected, piece);

            _unknownSpawnedPieceRecognized++;
        }
        
        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesSpawnedPieceNegativesNull))]
        public void NotRecognizeUnknownSpawnedPiece(string imageKey, IScreenshot screenshot)
        {
            _unknownSpawnedPiece++;

            // TODO: make tests with higher search distance!
            var searchHeight = 3;
            var piece = _extractor.ExtractSpawnedPiece(screenshot, searchHeight);

            Assert.Null(piece);

            _unknownSpawnedPieceRecognized++;
        }
        
        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesSpawnedPiecePositives))]
        public void RecognizeKnownSpawnedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            // TODO: implement?
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesTouchedPieces))]
        public void NotRecognizeKnownSpawnedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            // TODO: implement?
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesMovedPiece))]
        public void RecognizeMovedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected, Move move)
        {
            // TODO: implement?
        }

        [TestFixtureTearDown]
        public void Summary()
        {
            _logger.Info(BuildSummaryString("Test recognize unknown next piece", _nextPiece, _nextPieceRecognized));
            _logger.Info(BuildSummaryString("Test recognize unknown spawned piece", _unknownSpawnedPiece, _unknownSpawnedPieceRecognized));
            _logger.Info(BuildSummaryString("Test recognize known spawned piece", _knownSpawnedPiece, _knownSpawnedPieceRecognized));
            _logger.Info(BuildSummaryString("Test recognize moved piece", _movedPiece, _movedPieceRecognized));
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
