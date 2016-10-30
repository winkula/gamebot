using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;
using NLog;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class StatisticalMatcherTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private int _currentPiecesTotal;
        private int _currentPiecesRecognized;

        private int _nextPiecesTotal;
        private int _nextPiecesRecognized;

        // 0.65 seems to be a pretty accurate value. if we go deeper (0.6 for example), we get false positives (without binarization)
        // 0.7 seems to be good, when we use binarized templates
        private const double _probabilityThreshold = 0.8;
        
        private IMatcher _matcher;
        
        [TestFixtureSetUp]
        public void Init()
        {
            _matcher = new MorphologyMatcher();
            //_matcher = new TemplateMatcher();
        }

        [Ignore]
        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesCurrentPiecePositives))]
        public void PieceMatchingCurrentPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            _currentPiecesTotal++;
            var probabilityCurrentPiece = _matcher.GetProbability(screenshot, currentPieceExpected);
            _logger.Info($"PieceMatchingCurrentPiece: {probabilityCurrentPiece * 100.0:F}");

            var currentPieceFound = probabilityCurrentPiece >= _probabilityThreshold;

            if (currentPieceFound) { _currentPiecesRecognized++; }
            Assert.True(currentPieceFound);
        }
        
        //[Ignore]
        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesNextPiecePositives))]
        public void PieceMatchingNextPiecePositives(string imageKey, IScreenshot screenshot, Tetrimino nextPieceExpected)
        {
            _nextPiecesTotal++;
            var probabilityNextPiece = _matcher.GetProbabilityNextPiece(screenshot, nextPieceExpected);
            _logger.Info($"PieceMatchingNextPiecePositives: {probabilityNextPiece * 100.0:F}");

            var nextPieceFound = probabilityNextPiece >= _probabilityThreshold;

            if (nextPieceFound) _nextPiecesRecognized++;
            Assert.True(nextPieceFound);
        }

        //[Ignore]
        [TestCaseSource(typeof(ImageTestCaseFactory), nameof(ImageTestCaseFactory.TestCasesNextPieceNegativesNull))]
        public void PieceMatchingNextPieceNegatives(string imageKey, IScreenshot screenshot)
        {
            foreach (var tetrimino in Tetriminos.All)
            {
                _nextPiecesTotal++;
                var probabilityNextPiece = _matcher.GetProbabilityNextPiece(screenshot, tetrimino);
                _logger.Info($"PieceMatchingNextPieceNegatives: {probabilityNextPiece * 100.0:F}");

                var nextPieceFound = probabilityNextPiece >= _probabilityThreshold;

                if (nextPieceFound) _nextPiecesRecognized++;
                Assert.False(nextPieceFound);
            }
        }

        [TestFixtureTearDown]
        public void Summary()
        {
            _logger.Info($"Current piece: {_currentPiecesRecognized}/{_currentPiecesTotal} ({(double)_currentPiecesRecognized / _currentPiecesTotal * 100.0:F}%)");
            _logger.Info($"Next piece: {_nextPiecesRecognized}/{_nextPiecesTotal} ({(double)_nextPiecesRecognized / _nextPiecesTotal * 100.0:F}%)");
        }
    }
}
