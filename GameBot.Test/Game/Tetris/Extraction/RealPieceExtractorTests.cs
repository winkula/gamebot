using System;
using GameBot.Core.Data;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using NLog;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class RealPieceExtractorTests
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
        private PieceExtractor _pieceExtractor;

        [TestFixtureSetUp]
        public void Init()
        {
            _pieceMatcher = new PieceMatcher();
            _pieceExtractor = new PieceExtractor(_pieceMatcher);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesCurrentPiece))]
        public void PieceMatchingCurrentPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            _currentPiecesTotal++;
            var probabilityCurrentPiece = _pieceMatcher.GetProbability(screenshot, currentPieceExpected);

            var currentPieceFound = probabilityCurrentPiece >= _probabilityThreshold;

            if (currentPieceFound) { _currentPiecesRecognized++; }
            Assert.True(currentPieceFound);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesNextPiece))]
        public void PieceMatchingNextPiece(string imageKey, IScreenshot screenshot, Tetromino nextPieceExpected)
        {
            _nextPiecesTotal++;
            var probabilityNextPiece = _pieceMatcher.GetProbability(screenshot, new Piece(nextPieceExpected, 0, TetrisConstants.NextPieceTemplateTileCoordinates.X, TetrisConstants.NextPieceTemplateTileCoordinates.Y));

            var nextPieceFound = probabilityNextPiece >= _probabilityThreshold;

            if (nextPieceFound) _nextPiecesRecognized++;
            Assert.True(nextPieceFound);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesNextPiece))]
        public void RecognizeNextPiece(string imageKey, IScreenshot screenshot, Tetromino nextPieceExpected)
        {
            var tetromino = _pieceExtractor.ExtractNextPieceFuzzy(screenshot, _probabilityThreshold);

            Assert.AreEqual(nextPieceExpected, tetromino);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesNextPieceNull))]
        public void NotRecognizeNextPiece(string imageKey, IScreenshot screenshot)
        {
            var tetromino = _pieceExtractor.ExtractNextPieceFuzzy(screenshot, _probabilityThreshold);

            Assert.Null(tetromino);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesSpawnedPiece))]
        public void RecognizeSpawnedPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            // TODO: make tests with higher search distance!
            var maxFallingDistance = currentPieceExpected.FallHeight;
            var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, maxFallingDistance, _probabilityThreshold);

            Assert.AreEqual(currentPieceExpected, result.Item1);
            Assert.GreaterOrEqual(result.Item2, _probabilityThreshold);
            Assert.LessOrEqual(result.Item2, 1.0);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesSpawnedPieceNull))]
        public void NotRecognizeSpawnedPiece(string imageKey, IScreenshot screenshot)
        {
            const int maxFallingDistance = 10;
            var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, maxFallingDistance, _probabilityThreshold);
            
            Assert.Null(result.Item1);
            Assert.AreEqual(0.0, result.Item2);
        }
        
        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesCurrentPiece))]
        public void RecognizeKnownPiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            var result = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, currentPieceExpected, 0, _probabilityThreshold);

            Assert.AreEqual(currentPieceExpected, result.Item1);
            Assert.GreaterOrEqual(result.Item2, _probabilityThreshold);
            Assert.LessOrEqual(result.Item2, 1.0);
        }
        
        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesCurrentPiece))]
        public void RecognizeKnownPieceNotMoved(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            // generate pseudo random move
            Move move = (Move) (Math.Abs(currentPieceExpected.GetHashCode()) % 
                (currentPieceExpected.Tetromino == Tetromino.O ? 2 : 4));
            var pieceMoved = new Piece(currentPieceExpected).Apply(move);

            var resultBefore = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, currentPieceExpected, 0, _probabilityThreshold);
            var resultMoved = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, pieceMoved, 0, _probabilityThreshold);
            
            Assert.AreNotEqual(resultBefore.Item1, resultMoved.Item1);
            Assert.Greater(resultBefore.Item2, resultMoved.Item2);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesCurrentPiece))]
        public void RecognizePiece(string imageKey, IScreenshot screenshot, Piece currentPieceExpected)
        {
            var maxFallingDistance = currentPieceExpected.FallHeight;
            var result = _pieceExtractor.ExtractPieceFuzzy(screenshot, maxFallingDistance, _probabilityThreshold);

            Assert.AreEqual(currentPieceExpected, result.Item1);
            Assert.GreaterOrEqual(result.Item2, _probabilityThreshold);
            Assert.LessOrEqual(result.Item2, 1.0);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesCurrentPieceNull))]
        public void NotRecognizePiece(string imageKey, IScreenshot screenshot)
        {
            const int maxFallingDistance = 10;
            var result = _pieceExtractor.ExtractPieceFuzzy(screenshot, maxFallingDistance, _probabilityThreshold);

            Assert.Null(result.Item1);
            Assert.AreEqual(0.0, result.Item2);
        }

        [TestFixtureTearDown]
        public void Summary()
        {
            _logger.Info($"Current piece: {_currentPiecesRecognized}/{_currentPiecesTotal} ({(double)_currentPiecesRecognized / _currentPiecesTotal * 100.0:F}%)");
            _logger.Info($"Next piece: {_nextPiecesRecognized}/{_nextPiecesTotal} ({(double)_nextPiecesRecognized / _nextPiecesTotal * 100.0:F}%)");
        }
    }
}
