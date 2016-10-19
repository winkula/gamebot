using System.Text;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using NLog;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class StatisticalPieceExtractorTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        // 0.65 seems to be a pretty accurate value. if we go deeper (0.6 for example), we get false positives (without binarization)
        // 0.7 seems to be good, when we use binarized templates
        private const double _probabilityThreshold = 0.7;

        private int _nextPiece;
        private int _nextPieceRecognized;
        private int _unknownSpawnedPiece;
        private int _unknownSpawnedPieceRecognized;
        private int _knownSpawnedPiece;
        private int _knownSpawnedPieceRecognized;
        private int _movedPiece;
        private int _movedPieceRecognized;

        private PieceMatcher _pieceMatcher;
        private PieceExtractor _pieceExtractor;

        [TestFixtureSetUp]
        public void Init()
        {
            _pieceMatcher = new PieceMatcher();
            _pieceExtractor = new PieceExtractor(_pieceMatcher);
        }
        
        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesNextPiecePositives))]
        public void RecognizeNextPiece(string imageKey, IScreenshot screenshot, Tetromino nextPieceExpected)
        {
            _nextPiece++;

            var result = _pieceExtractor.ExtractNextPieceFuzzy(screenshot);

            Assert.True(result.IsAccepted(_probabilityThreshold));
            Assert.AreEqual(nextPieceExpected, result.Result);
            Assert.GreaterOrEqual(result.Probability, _probabilityThreshold);
            Assert.LessOrEqual(result.Probability, 1.0);

            _nextPieceRecognized++;
        }
        
        // not relevant because not an issue in A-Type mode
        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesNextPieceNegativesNull))]
        public void NotRecognizeNextPiece(string imageKey, IScreenshot screenshot)
        {
            var result = _pieceExtractor.ExtractNextPieceFuzzy(screenshot);

            Assert.True(result.IsRejected(_probabilityThreshold));
            Assert.GreaterOrEqual(result.Probability, 0.0);
            Assert.Less(result.Probability, _probabilityThreshold);
        }

        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesSpawnedPiecePositives))]
        public void RecognizeUnknownSpawnedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            _unknownSpawnedPiece++;

            // TODO: make tests with higher search distance!
            var maxFallingDistance = currentPieceExpected.FallHeight;
            var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, maxFallingDistance);

            Assert.True(result.IsAccepted(_probabilityThreshold));
            Assert.AreEqual(currentPieceExpected, result.Result);
            Assert.GreaterOrEqual(result.Probability, _probabilityThreshold);
            Assert.LessOrEqual(result.Probability, 1.0);

            _unknownSpawnedPieceRecognized++;
        }

        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesSpawnedPieceNegativesNull))]
        public void NotRecognizeUnknownSpawnedPiece(string imageKey, IScreenshot screenshot)
        {
            _unknownSpawnedPiece++;

            // TODO: make tests with higher search distance!
            var maxFallingDistance = 3;
            var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, maxFallingDistance);

            Assert.True(result.IsRejected(_probabilityThreshold));
            Assert.GreaterOrEqual(result.Probability, 0.0);
            Assert.Less(result.Probability, _probabilityThreshold);

            _unknownSpawnedPieceRecognized++;
        }

        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesSpawnedPiecePositives))]
        public void RecognizeKnownSpawnedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            _knownSpawnedPiece++;

            // TODO: make tests with higher search distance!
            var maxFallingDistance = currentPieceExpected.FallHeight;
            var result = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, currentPieceExpected, maxFallingDistance);

            Assert.True(result.IsAccepted(_probabilityThreshold));
            Assert.AreEqual(currentPieceExpected, result.Result);
            Assert.GreaterOrEqual(result.Probability, _probabilityThreshold);
            Assert.LessOrEqual(result.Probability, 1.0);

            _knownSpawnedPieceRecognized++;
        }

        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesTouchedPieces))]
        public void NotRecognizeKnownSpawnedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            _knownSpawnedPiece++;

            var spawned = new Piece(currentPieceExpected.Tetromino);

            // TODO: make tests with higher search distance!
            var maxFallingDistance = currentPieceExpected.FallHeight;
            var result = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, spawned, maxFallingDistance);

            Assert.True(result.IsRejected(_probabilityThreshold));
            Assert.GreaterOrEqual(result.Probability, 0.0);
            Assert.Less(result.Probability, _probabilityThreshold);

            _knownSpawnedPieceRecognized++;
        }

        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesMovedPiece))]
        public void RecognizeMovedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected, Move move)
        {
            _movedPiece++;

            // TODO: make tests with higher search distance!
            var maxFallingDistance = currentPieceExpected.FallHeight;

            var pieceTrue = new Piece(currentPieceExpected.Tetromino, currentPieceExpected.Orientation, currentPieceExpected.X);
            var pieceFalse = new Piece(pieceTrue).Apply(move);

            var resultTrue = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, pieceTrue, maxFallingDistance);
            var resultFalse = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, pieceFalse, maxFallingDistance);
            
            Assert.AreNotEqual(resultTrue.Result, resultFalse.Result);
            Assert.Greater(resultTrue.Probability, resultFalse.Probability);

            //_logger.Info($"Probability: {resultTrue.Probability}, {resultFalse.Probability}");
            _logger.Info($"{resultTrue.Result} | {resultFalse.Result}");
            _logger.Info($"Probability difference: {resultTrue.Probability - resultFalse.Probability}");

            _movedPieceRecognized++;
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
