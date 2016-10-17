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
    public class ExtractionFromImagesTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private int _currentPiecesTotal;
        private int _currentPiecesRecognized;
        private int _nextPiecesTotal;
        private int _nextPiecesRecognized;

        // 0.6 seems to be a pretty accurate value. if we go deeper (0.5 for example), we get false positives.
        private const double _probabilityThreshold = 0.6;

        private PieceMatcher _pieceMatcher;
        private PieceExtractor _pieceExtractor;

        [TestFixtureSetUp]
        public void Init()
        {
            _pieceMatcher = new PieceMatcher();
            _pieceExtractor = new PieceExtractor(_pieceMatcher);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesScreenshot))]
        public void PieceMatching(IScreenshot screenshot, Piece currentPieceExpected, Tetromino? nextPieceExpected)
        {
            if (currentPieceExpected != null)
            {
                _currentPiecesTotal++;
                var probabilityCurrentPiece = _pieceMatcher.GetProbability(screenshot, currentPieceExpected);
                bool currentPieceFound = probabilityCurrentPiece >= _probabilityThreshold;
                if (currentPieceFound) _currentPiecesRecognized++;
            }

            if (nextPieceExpected.HasValue)
            {
                _nextPiecesTotal++;
                var probabilityNextPiece = _pieceMatcher.GetProbability(screenshot, new Piece(nextPieceExpected.Value, 0, TetrisConstants.NextPieceTemplateTileCoordinates.X, TetrisConstants.NextPieceTemplateTileCoordinates.Y));
                bool nextPieceFound = probabilityNextPiece >= _probabilityThreshold;
                if (nextPieceFound) _nextPiecesRecognized++;
            }

            Assert.True(true);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesScreenshot))]
        public void RecognizeNextPiece(IScreenshot screenshot, Piece currentPieceExpected, Tetromino? nextPieceExpected)
        {
            var tetromino = _pieceExtractor.ExtractNextPieceFuzzy(screenshot, _probabilityThreshold);

            Assert.AreEqual(nextPieceExpected, tetromino);
        }
        
        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesScreenshot))]
        public void RecognizeSpawnedPiece(IScreenshot screenshot, Piece currentPieceExpected, Tetromino? nextPieceExpected)
        {
            if (currentPieceExpected != null && !currentPieceExpected.IsUntouched)
            {
                Assert.Ignore("Current piece is not untouched");
            }
            
            if (currentPieceExpected != null)
            {
                var maxFallingDistance = Math.Abs(currentPieceExpected.Y);
                var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, maxFallingDistance, _probabilityThreshold);

                Assert.AreEqual(currentPieceExpected, result.Item1);
                Assert.GreaterOrEqual(result.Item2, _probabilityThreshold);
                Assert.LessOrEqual(result.Item2, 1.0);
            }
            else
            {
                // no piece visible on screen

                var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, 10, _probabilityThreshold);

                Assert.Null(result.Item1);
                Assert.AreEqual(0.0, result.Item2);
            }
        }


        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCasesScreenshot))]
        public void RecognizePiece(IScreenshot screenshot, Piece currentPieceExpected, Tetromino? nextPieceExpected)
        {
            if (currentPieceExpected != null)
            {
                var maxFallingDistance = Math.Abs(currentPieceExpected.Y);
                var result = _pieceExtractor.ExtractPieceFuzzy(screenshot, maxFallingDistance, _probabilityThreshold);

                Assert.AreEqual(currentPieceExpected, result.Item1);
                Assert.GreaterOrEqual(result.Item2, _probabilityThreshold);
                Assert.LessOrEqual(result.Item2, 1.0);
            }
            else
            {
                // no piece visible on screen

                var result = _pieceExtractor.ExtractPieceFuzzy(screenshot, 10, _probabilityThreshold);

                Assert.Null(result.Item1);
                Assert.AreEqual(0.0, result.Item2);
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
