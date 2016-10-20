using GameBot.Core.Data;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using NLog;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class StatisticalPieceMatcherTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private int _currentPiecesTotal;
        private int _currentPiecesRecognized;

        private int _nextPiecesTotal;
        private int _nextPiecesRecognized;

        // 0.65 seems to be a pretty accurate value. if we go deeper (0.6 for example), we get false positives (without binarization)
        // 0.7 seems to be good, when we use binarized templates
        private const double _probabilityThreshold = 0.7;

        private PieceMatcher _pieceMatcher;

        [TestFixtureSetUp]
        public void Init()
        {
            _pieceMatcher = new PieceMatcher();
        }

        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesCurrentPiecePositives))]
        public void PieceMatchingCurrentPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            _currentPiecesTotal++;
            var probabilityCurrentPiece = _pieceMatcher.GetProbability(screenshot, currentPieceExpected);
            _logger.Info($"Probability: {probabilityCurrentPiece * 100.0:F}");

            var currentPieceFound = probabilityCurrentPiece >= _probabilityThreshold;

            if (currentPieceFound) { _currentPiecesRecognized++; }
            Assert.True(currentPieceFound);
        }

        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesNextPiecePositives))]
        public void PieceMatchingNextPiecePositives(string imageKey, IScreenshot screenshot, Tetrimino nextPieceExpected)
        {
            _nextPiecesTotal++;
            var probabilityNextPiece = _pieceMatcher.GetProbability(screenshot, new Piece(nextPieceExpected, 0, TetrisConstants.NextPieceTemplateTileCoordinates.X, TetrisConstants.NextPieceTemplateTileCoordinates.Y));
            _logger.Info($"Probability: {probabilityNextPiece * 100.0:F}");

            var nextPieceFound = probabilityNextPiece >= _probabilityThreshold;

            if (nextPieceFound) _nextPiecesRecognized++;
            Assert.True(nextPieceFound);
        }

        [TestFixtureTearDown]
        public void Summary()
        {
            _logger.Info($"Current piece: {_currentPiecesRecognized}/{_currentPiecesTotal} ({(double)_currentPiecesRecognized / _currentPiecesTotal * 100.0:F}%)");
            _logger.Info($"Next piece: {_nextPiecesRecognized}/{_nextPiecesTotal} ({(double)_nextPiecesRecognized / _nextPiecesTotal * 100.0:F}%)");
        }
    }
}
