﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Configuration;
using GameBot.Core.Data;
using GameBot.Core.Exceptions;
using GameBot.Engine.Physical.Quantizers;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Extractors;
using NLog;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class ExtractionComparisonTests
    {
        private static readonly Logger _loggerResults = LogManager.GetLogger("Tests");
        private static readonly Logger _loggerFails = LogManager.GetLogger("Fails");

        private Random _random;
        private Quantizer _quantizer1;

        [TestFixtureSetUp]
        public void Init()
        {
            _random = new Random(123);
            _quantizer1 = new Quantizer(new AppSettingsConfig());
        }

        [Test]
        public void Compare()
        {
            var extractors = new IExtractor[] { new BlockBasedExtractor(), new PieceBasedExtractor(), new MorphologyExtractor() };
            var quantizers = new ICalibrateableQuantizer[] { _quantizer1 };
            var shifts = new[] { 0.0, 1.0, 2.0, 3.0, 4.0, 8.0, 16.0 };

            var sb = new StringBuilder();
            sb.AppendLine("Quantizer;Extractor;Method;Shift;Total;Positive;Accuracy");

            foreach (var quantizer in quantizers)
            {
                foreach (var extractor in extractors)
                {
                    foreach (var shift in shifts)
                    {
                        var input = new ExtractionComparisonInput(extractor, quantizer);

                        // run tests...
                        var results1 = RecognizeNextPiece(input, shift);
                        sb.AppendLine($"{quantizer.GetType().Name};{extractor.GetType().Name};RecognizeNextPiece;{shift:F};{results1.Total};{results1.Recognized};{100.0 * results1.Recognized / results1.Total:F}");

                        var results2 = RecognizeCurrentPieceUnknown(input, shift);
                        sb.AppendLine($"{quantizer.GetType().Name};{extractor.GetType().Name};RecognizeCurrentPieceUnknown;{shift:F};{results2.Total};{results2.Recognized};{100.0 * results2.Recognized / results2.Total:F}");

                        var results3 = RecognizeCurrentPieceKnown(input, shift);
                        sb.AppendLine($"{quantizer.GetType().Name};{extractor.GetType().Name};RecognizeCurrentPieceKnown;{shift:F};{results3.Total};{results3.Recognized};{100.0 * results3.Recognized / results3.Total:F}");

                        var results4 = RecognizeMove(input, shift);
                        sb.AppendLine($"{quantizer.GetType().Name};{extractor.GetType().Name};RecognizeMove;{shift:F};{results4.Total};{results4.Recognized};{100.0 * results4.Recognized / results4.Total:F}");
                    }
                }
            }

            _loggerResults.Info(sb.ToString());
        }

        private ExtractionComparisonResults RecognizeNextPiece(ExtractionComparisonInput input, double shift)
        {
            var results = new ExtractionComparisonResults();

            var tests = ImageTestCaseFactory.Data
                .Where(x => x.NextPiece.HasValue)
                .ToList();

            foreach (var test in tests)
            {
                var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, shift);

                var nextPiece = input.Extractor.ExtractNextPiece(screenshot);

                results.Total++;
                if (test.NextPiece == nextPiece)
                {
                    results.Recognized++;
                }
                else
                {
                    _loggerFails.Warn($"RecognizeNextPiece. Recognized: {nextPiece}, Real: {test.NextPiece}. Testcase {test.ImageKey}. Shift {shift:F}");
                }
            }

            return results;
        }

        private ExtractionComparisonResults RecognizeCurrentPieceUnknown(ExtractionComparisonInput input, double shift)
        {
            var results = new ExtractionComparisonResults();

            var positiveTests = ImageTestCaseFactory.Data
                .Where(x => x.Piece != null)
                .Where(x => x.Piece.IsUntouched)
                .ToList();

            foreach (var test in positiveTests)
            {
                var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, shift);

                var currentPiece = input.Extractor.ExtractCurrentPiece(screenshot, null, test.Piece.FallHeight);

                results.Total++;
                if (currentPiece != null && currentPiece.Tetrimino == test.Piece.Tetrimino)
                {
                    results.Recognized++;
                }
            }

            var negativeTests = ImageTestCaseFactory.Data
                .Where(x =>
                    x.Piece == null ||
                    x.Piece.FallHeight >= 3 ||
                    (!x.Piece.IsUntouched && x.Piece.FallHeight >= 3))
                .ToList();

            foreach (var test in negativeTests)
            {
                var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, shift);

                var currentPiece = input.Extractor.ExtractCurrentPiece(screenshot, null, 0);

                results.Total++;
                if (currentPiece == null)
                {
                    results.Recognized++;
                }
                else
                {
                    _loggerFails.Warn($"RecognizeCurrentPieceUnknown. Recognized: {currentPiece}, Real: {test.Piece}. Testcase {test.ImageKey}. Shift {shift:F}");
                }
            }

            return results;
        }

        private ExtractionComparisonResults RecognizeCurrentPieceKnown(ExtractionComparisonInput input, double shift)
        {
            var results = new ExtractionComparisonResults();

            var tests = ImageTestCaseFactory.Data
                .Where(x => x.Piece != null)
                .Where(x => x.Piece.IsUntouched)
                .ToList();

            foreach (var test in tests)
            {
                var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, shift);

                var currentPiece = input.Extractor.ExtractCurrentPiece(screenshot, test.Piece.Tetrimino, test.Piece.FallHeight);

                results.Total++;
                if (currentPiece != null && currentPiece.Tetrimino == test.Piece.Tetrimino)
                {
                    results.Recognized++;
                }
                else
                {
                    _loggerFails.Warn($"RecognizeCurrentPieceKnown. Recognized: {currentPiece}, Real: {test.Piece}. Testcase {test.ImageKey}. Shift {shift:F}");
                }
            }

            var negativeTests = ImageTestCaseFactory.Data
                .Where(x =>
                    x.Piece == null ||
                    x.Piece.FallHeight >= 3 ||
                    (!x.Piece.IsUntouched && x.Piece.FallHeight >= 3))
                .ToList();

            foreach (var test in negativeTests)
            {
                var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, shift);

                var currentPiece = input.Extractor.ExtractCurrentPiece(screenshot, null, 0);

                results.Total++;
                if (currentPiece == null)
                {
                    results.Recognized++;
                }
                else
                {
                    _loggerFails.Warn($"RecognizeCurrentPieceKnown. Recognized: {currentPiece}, Real: {test.Piece}. Testcase {test.ImageKey}. Shift {shift:F}");
                }
            }

            return results;
        }

        private ExtractionComparisonResults RecognizeMove(ExtractionComparisonInput input, double shift)
        {
            var results = new ExtractionComparisonResults();

            var tests = ImageTestCaseFactory.Data
                .Where(x => x.Piece != null)
                .ToList();

            foreach (var test in tests)
            {
                var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, shift);

                var move = GetValidMove(test);
                if (!move.HasValue) continue;

                bool moved;
                var pieceOriginNotMoved = new Piece(test.Piece.Tetrimino, test.Piece.Orientation, test.Piece.X);
                var pieceOriginMoved = new Piece(pieceOriginNotMoved).Apply(move.Value);
                var recognizedPiece = input.Extractor.ExtractMovedPiece(screenshot, pieceOriginNotMoved, move.Value, test.Piece.FallHeight, out moved);

                if (pieceOriginMoved.Equals(pieceOriginNotMoved)) throw new Exception("pieces must be different!");

                results.Total++;
                if (recognizedPiece != null && recognizedPiece.Equals(test.Piece) && !moved)
                {
                    results.Recognized++;
                }
                else
                {
                    _loggerFails.Warn($"RecognizeMove. Recognized: {recognizedPiece}, Real: {test.Piece}. Testcase {test.ImageKey}. Shift {shift:F}");
                }
            }

            return results;
        }

        private Move? GetValidMove(ImageTestCaseFactory.TestData test)
        {
            var validMoves = (test.Piece.Tetrimino == Tetrimino.O ? 
                new[] { Move.Left, Move.Right } : 
                new[] { Move.Left, Move.Right, Move.Rotate, Move.RotateCounterclockwise })
            .OrderBy(x => Guid.NewGuid())
            .ToArray();

            foreach (var move in validMoves)
            {
                var gamestate = new GameState(new Piece(test.Piece), test.NextPiece);

                try
                {
                    switch (move)
                    {
                        case Move.Left:
                            gamestate.Left();
                            break;
                        case Move.Right:
                            gamestate.Right();
                            break;
                        case Move.Rotate:
                            gamestate.Rotate();
                            break;
                        case Move.RotateCounterclockwise:
                            gamestate.RotateCounterclockwise();
                            break;
                    }
                    return move;
                }
                catch (GameOverException)
                {
                    // this move is not valid, ignore
                }
            }

            return null;
        }

        private IScreenshot GetScreenshot(ICalibrateableQuantizer quantizer, IImage image, Point[] keypoints, double shift)
        {
            var shiftedKeypoints = ShiftKeypoints(keypoints, shift);
            quantizer.Calibrate(shiftedKeypoints);

            var quantized = quantizer.Quantize(image);
            var screenshot = new EmguScreenshot(quantized, TimeSpan.Zero);

            return screenshot;
        }

        private Point[] ShiftKeypoints(Point[] keypoints, double shift)
        {
            var newPoints = new List<Point>();

            foreach (var keypoint in keypoints)
            {
                int newX = keypoint.X + (int)(shift * 2 * _random.NextDouble() - 1);
                int newY = keypoint.Y + (int)(shift * 2 * _random.NextDouble() - 1);

                newPoints.Add(new Point(newX, newY));
            }

            return newPoints.ToArray();
        }
    }

    public class ExtractionComparisonInput
    {
        public IExtractor Extractor { get; }
        public ICalibrateableQuantizer Quantizer { get; }

        public ExtractionComparisonInput(IExtractor extractor, ICalibrateableQuantizer quantizer)
        {
            Extractor = extractor;
            Quantizer = quantizer;
        }
    }

    public class ExtractionComparisonResults
    {
        public int Total { get; set; }
        public int Recognized { get; set; }
    }
}
