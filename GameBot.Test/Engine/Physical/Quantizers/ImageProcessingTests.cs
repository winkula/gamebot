﻿using System;
using System.Collections;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using GameBot.Core.Configuration;
using GameBot.Core.Data;
using GameBot.Engine.Physical.Quantizers;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using NLog;
using NUnit.Framework;

namespace GameBot.Test.Engine.Physical.Quantizers
{
    [TestFixture]
    public class ImageProcessingTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private int _currentPiecesTotal;
        private int _currentPiecesRecognized;
        private int _nextPiecesTotal;
        private int _nextPiecesRecognized;

        // 0.6 seems to be a pretty accurate value. if we fo deeper (0.5 for example) we get false positives.
        private const double _probabilityThreshold = 0.6;

        private IConfig _config;
        private ICalibrateableQuantizer _quantizer;
        private PieceMatcher _pieceMatcher;
        private PieceExtractor _pieceExtractor;

        [TestFixtureSetUp]
        public void Init()
        {
            _currentPiecesTotal = 0;
            _currentPiecesRecognized = 0;
            _nextPiecesTotal = 0;
            _nextPiecesRecognized = 0;

            _config = new AppSettingsConfig();
            _quantizer = new Quantizer(_config);
            _pieceMatcher = new PieceMatcher();
            _pieceExtractor = new PieceExtractor(_pieceMatcher);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCases))]
        public void PieceMatching(string imagePath, Point[] keypoints, Piece currentPieceExpected, Tetromino? nextPieceExpected)
        {
            _quantizer.Calibrate(keypoints);

            var image = new Mat(imagePath, LoadImageType.AnyColor);
            var quantizedImage = _quantizer.Quantize(image);
            var screenshot = new EmguScreenshot(quantizedImage, TimeSpan.Zero);

            /*
            CvInvoke.Imshow("original", image);
            CvInvoke.Imshow("quantized", quantizedImage);
            CvInvoke.WaitKey();
            */

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

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCases))]
        public void RecognizeNextPiece(string imagePath, Point[] keypoints, Piece currentPieceExpected, Tetromino? nextPieceExpected)
        {
            _quantizer.Calibrate(keypoints);

            var image = new Mat(imagePath, LoadImageType.AnyColor);
            var quantizedImage = _quantizer.Quantize(image);
            var screenshot = new EmguScreenshot(quantizedImage, TimeSpan.Zero);

            var tetromino = _pieceExtractor.ExtractNextPieceFuzzy(screenshot, _probabilityThreshold);

            Assert.AreEqual(nextPieceExpected, tetromino);
        }

        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCases))]
        public void RecognizeSpawnedPiece(string imagePath, Point[] keypoints, Piece currentPieceExpected, Tetromino? nextPieceExpected)
        {
            if (currentPieceExpected != null && !currentPieceExpected.IsUntouched)
            {
                Assert.Ignore("Current piece is not untouched");
            }

            _quantizer.Calibrate(keypoints);

            var image = new Mat(imagePath, LoadImageType.AnyColor);
            var quantizedImage = _quantizer.Quantize(image);
            var screenshot = new EmguScreenshot(quantizedImage, TimeSpan.Zero);

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


        [TestCaseSource(typeof(TestImageFactory), nameof(TestImageFactory.TestCases))]
        public void RecognizePiece(string imagePath, Point[] keypoints, Piece currentPieceExpected, Tetromino? nextPieceExpected)
        {
            _quantizer.Calibrate(keypoints);

            var image = new Mat(imagePath, LoadImageType.AnyColor);
            var quantizedImage = _quantizer.Quantize(image);
            var screenshot = new EmguScreenshot(quantizedImage, TimeSpan.Zero);

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

    public class TestImageFactory
    {
        private static readonly Point[][] _keypoints =
        {
            new [] { new Point(583,361), new Point(206,358), new Point(569,59), new Point(229,59) }, // series 00
            new [] { new Point(585,360), new Point(207,359), new Point(571,58), new Point(228,57) }, // series 01
            new [] { new Point(593,357), new Point(206,354), new Point(574,53), new Point(230,53) }, // series 02
            new [] { new Point(593,357), new Point(206,354), new Point(574,53), new Point(230,53) } // series 03
        };

        private static readonly object[][] _data =
        {
            new object[] { "0000", new Piece(Tetromino.T), Tetromino.J },
            new object[] { "0001", new Piece(Tetromino.J), Tetromino.S },
            new object[] { "0002", new Piece(Tetromino.S), Tetromino.S },
            new object[] { "0003", new Piece(Tetromino.S), Tetromino.O },
            new object[] { "0004", new Piece(Tetromino.O), Tetromino.J },
            new object[] { "0005", new Piece(Tetromino.J), Tetromino.T },
            new object[] { "0006", new Piece(Tetromino.L), Tetromino.J },
            new object[] { "0007", new Piece(Tetromino.J), Tetromino.L },
            new object[] { "0008", new Piece(Tetromino.L), Tetromino.L },
            new object[] { "0009", new Piece(Tetromino.L), Tetromino.T },
            new object[] { "0010", new Piece(Tetromino.T), Tetromino.T },
            new object[] { "0011", new Piece(Tetromino.Z), Tetromino.J },

            new object[] { "0100", new Piece(Tetromino.O), Tetromino.I },
            new object[] { "0101", new Piece(Tetromino.I), Tetromino.T },
            new object[] { "0102", new Piece(Tetromino.T), Tetromino.O },
            new object[] { "0103", new Piece(Tetromino.T, 0, 3, -5), Tetromino.O },
            new object[] { "0104", new Piece(Tetromino.T, 0, 3, -10), Tetromino.O },
            new object[] { "0105", new Piece(Tetromino.O, 0, 4, -4), Tetromino.J },
            new object[] { "0106", new Piece(Tetromino.O, 0, 4, -7), Tetromino.J },
            new object[] { "0107", new Piece(Tetromino.J).Fall(2), null },

            new object[] { "0200", new Piece(Tetromino.T).Fall(2), Tetromino.L },
            new object[] { "0201", new Piece(Tetromino.T, 0, -1, -9), Tetromino.L },
            new object[] { "0202", new Piece(Tetromino.L, 0, -3, -4), Tetromino.O },
            new object[] { "0203", new Piece(Tetromino.O).Fall(3), Tetromino.J },
            new object[] { "0204", new Piece(Tetromino.O, 0, -2, -10), Tetromino.J },
            new object[] { "0205", new Piece(Tetromino.J, 0, 4, -6), Tetromino.O },
            new object[] { "0206", new Piece(Tetromino.J, 0, 4, -8), Tetromino.O },
            new object[] { "0207", new Piece(Tetromino.O).Fall(2), Tetromino.T },
            new object[] { "0208", new Piece(Tetromino.O, 0, 2, -5), Tetromino.T },
            new object[] { "0209", new Piece(Tetromino.T, 0, 4, -4), Tetromino.L },
            new object[] { "0210", new Piece(Tetromino.L, 0, 4, -4), Tetromino.J },
            new object[] { "0211", new Piece(Tetromino.S).Fall(4), null },
            new object[] { "0212", new Piece(Tetromino.S).Fall(), null },

            new object[] { "0300", null, null }
        };

        public static IEnumerable TestCases
        {
            get
            {
                foreach (var data in _data)
                {
                    var imageKey = (string)data[0];
                    var imagePath = $"Images/test{imageKey}.jpg";
                    var keypoints = _keypoints[int.Parse(imageKey.Substring(0, 2))];
                    var currentPiece = data[1];
                    var nextPiece = data[2];

                    yield return new TestCaseData(imagePath, keypoints, currentPiece, nextPiece);
                }
            }
        }

        public static IEnumerable TestCasesScreenshot
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
