using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        private static readonly Logger _loggerComparison = LogManager.GetLogger("ExtractionComparison");
        private static readonly Logger _loggerDetails = LogManager.GetLogger("ExtractionDetails");
        private static readonly Logger _loggerFails = LogManager.GetLogger("Fails");

        private const int _multiplySamplesBy = 10;

        private IConfig _config;
        private Random _random;
        private Quantizer _quantizer1;

        [TestFixtureSetUp]
        public void Init()
        {
            _config = new AppSettingsConfig();
            _random = new Random(123);
            _quantizer1 = new Quantizer(_config);
        }

        [Test]
        public void Compare()
        {
            var extractors = new IExtractor[] {/* new BlockBasedExtractor(),*//* new PieceBasedExtractor(),*/ new MorphologyExtractor(_config) };
            var quantizers = new IQuantizer[] { _quantizer1 };
            var shifts = new[] { 0, 1, 2, 3, 4, 8, 16 };
            
            var results = new List<ExtractionComparisonResult>();
            foreach (var quantizer in quantizers)
            {
                foreach (var extractor in extractors)
                {
                    foreach (var shift in shifts)
                    {
                        var input = new ExtractionComparisonInput(extractor, quantizer, shift);

                        // run tests...
                        AddResults(results, RecognizeNextPiece(input));
                        AddResults(results, RecognizeCurrentPieceUnknown(input));
                        AddResults(results, RecognizeCurrentPieceKnown(input));
                        AddResults(results, RecognizeMove(input));
                    }
                }
            }

            if (extractors.Length > 1)
            {
                _loggerComparison.Info("Shift;BlockBasedExtractor;PieceBasedExtractor;MorphologyExtractor");
                foreach (var grouping in results.GroupBy(x => x.Shift).ToList())
                {
                    _loggerComparison.Info($"{grouping.Key};" +
                                           $"{grouping.Where(x => x.ExtractorName == "BlockBasedExtractor").Select(x => x.Accuracy).Average():F};" +
                                           $"{grouping.Where(x => x.ExtractorName == "PieceBasedExtractor").Select(x => x.Accuracy).Average():F};" +
                                           $"{grouping.Where(x => x.ExtractorName == "MorphologyExtractor").Select(x => x.Accuracy).Average():F}");
                }
            }
            if (extractors.Length == 1)
            {
                _loggerDetails.Info("Shift;RecognizeNextPiece;RecognizeCurrentPieceUnknown;RecognizeCurrentPieceKnown;RecognizeMove");
                foreach (var grouping in results.GroupBy(x => x.Shift).ToList())
                {
                    _loggerDetails.Info($"{grouping.Key};" +
                                        $"{grouping.Where(x => x.MethodName == "RecognizeNextPiece").Select(x => x.Accuracy).Average():F};" +
                                        $"{grouping.Where(x => x.MethodName == "RecognizeCurrentPieceUnknown").Select(x => x.Accuracy).Average():F};" +
                                        $"{grouping.Where(x => x.MethodName == "RecognizeCurrentPieceKnown").Select(x => x.Accuracy).Average():F};" +
                                        $"{grouping.Where(x => x.MethodName == "RecognizeMove").Select(x => x.Accuracy).Average():F}");
                }
            }
        }

        private void AddResults(IList<ExtractionComparisonResult> results, ExtractionComparisonResult result)
        {
            result.Calculate();
            results.Add(result);
        }

        private ExtractionComparisonResult RecognizeNextPiece(ExtractionComparisonInput input)
        {
            var results = new ExtractionComparisonResult(input, nameof(RecognizeNextPiece));

            var tests = ImageTestCaseFactory.Data
                .Where(x => x.NextPiece.HasValue)
                .ToList();

            for (int i = 0; i < _multiplySamplesBy; i++)
            {
                foreach (var test in tests)
                {
                    var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, input.Shift);

                    var nextPiece = input.Extractor.ExtractNextPiece(screenshot);

                    results.Total++;
                    if (test.NextPiece == nextPiece)
                    {
                        results.Recognized++;
                    }
                    else
                    {
                        _loggerFails.Warn($"RecognizeNextPiece. Recognized: {nextPiece}, Real: {test.NextPiece}. Testcase {test.ImageKey}. Input {input}");
                    }
                }
            }

            return results;
        }

        private ExtractionComparisonResult RecognizeCurrentPieceUnknown(ExtractionComparisonInput input)
        {
            var results = new ExtractionComparisonResult(input, nameof(RecognizeCurrentPieceUnknown));

            var positiveTests = ImageTestCaseFactory.Data
                .Where(x => x.Piece != null)
                .Where(x => x.Piece.IsUntouched)
                .ToList();

            for (int i = 0; i < _multiplySamplesBy; i++)
            {
                foreach (var test in positiveTests)
                {
                    var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, input.Shift);

                    var currentPiece = input.Extractor.ExtractCurrentPiece(screenshot, null, test.Piece.FallHeight);

                    results.Total++;
                    if (currentPiece != null && currentPiece.Tetrimino == test.Piece.Tetrimino)
                    {
                        results.Recognized++;
                    }
                }
            }

            var negativeTests = ImageTestCaseFactory.Data
                .Where(x =>
                    x.Piece == null ||
                    x.Piece.FallHeight >= 3 ||
                    (!x.Piece.IsUntouched && x.Piece.FallHeight >= 3))
                .ToList();

            for (int i = 0; i < _multiplySamplesBy; i++)
            {
                foreach (var test in negativeTests)
                {
                    var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, input.Shift);

                    var currentPiece = input.Extractor.ExtractCurrentPiece(screenshot, null, 0);

                    results.Total++;
                    if (currentPiece == null)
                    {
                        results.Recognized++;
                    }
                    else
                    {
                        _loggerFails.Warn($"RecognizeCurrentPieceUnknown. Recognized: {currentPiece}, Real: {test.Piece}. Testcase {test.ImageKey}. Input {input}");
                    }
                }
            }

            return results;
        }

        private ExtractionComparisonResult RecognizeCurrentPieceKnown(ExtractionComparisonInput input)
        {
            var results = new ExtractionComparisonResult(input, nameof(RecognizeCurrentPieceKnown));

            var tests = ImageTestCaseFactory.Data
                .Where(x => x.Piece != null)
                .Where(x => x.Piece.IsUntouched)
                .ToList();

            for (int i = 0; i < _multiplySamplesBy; i++)
            {
                foreach (var test in tests)
                {
                    var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, input.Shift);

                    var currentPiece = input.Extractor.ExtractCurrentPiece(screenshot, test.Piece.Tetrimino,
                        test.Piece.FallHeight);

                    results.Total++;
                    if (currentPiece != null && currentPiece.Tetrimino == test.Piece.Tetrimino)
                    {
                        results.Recognized++;
                    }
                    else
                    {
                        _loggerFails.Warn($"RecognizeCurrentPieceKnown. Recognized: {currentPiece}, Real: {test.Piece}. Testcase {test.ImageKey}. Input {input}");
                    }
                }
            }

            var negativeTests = ImageTestCaseFactory.Data
                .Where(x =>
                    x.Piece == null ||
                    x.Piece.FallHeight >= 3 ||
                    (!x.Piece.IsUntouched && x.Piece.FallHeight >= 3))
                .ToList();

            for (int i = 0; i < _multiplySamplesBy; i++)
            {
                foreach (var test in negativeTests)
                {
                    var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, input.Shift);

                    var currentPiece = input.Extractor.ExtractCurrentPiece(screenshot, null, 0);

                    results.Total++;
                    if (currentPiece == null)
                    {
                        results.Recognized++;
                    }
                    else
                    {
                        _loggerFails.Warn($"RecognizeCurrentPieceKnown. Recognized: {currentPiece}, Real: {test.Piece}. Testcase {test.ImageKey}. Input {input}");
                    }
                }
            }

            return results;
        }

        private ExtractionComparisonResult RecognizeMove(ExtractionComparisonInput input)
        {
            var results = new ExtractionComparisonResult(input, nameof(RecognizeMove));

            var tests = ImageTestCaseFactory.Data
                .Where(x => x.Piece != null)
                .ToList();

            for (int i = 0; i < _multiplySamplesBy; i++)
            {
                foreach (var test in tests)
                {
                    var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, input.Shift);

                    var move = GetValidMove(test);
                    if (!move.HasValue) continue;

                    bool moved;
                    var pieceOriginNotMoved = new Piece(test.Piece.Tetrimino, test.Piece.Orientation, test.Piece.X);
                    var pieceOriginMoved = new Piece(pieceOriginNotMoved).Apply(move.Value);
                    var recognizedPiece = input.Extractor.ExtractMovedPiece(screenshot, pieceOriginNotMoved, move.Value,
                        test.Piece.FallHeight, out moved);

                    if (pieceOriginMoved.Equals(pieceOriginNotMoved)) throw new Exception("pieces must be different!");

                    results.Total++;
                    if (recognizedPiece != null && recognizedPiece.Equals(test.Piece) && !moved)
                    {
                        results.Recognized++;
                    }
                    else
                    {
                        _loggerFails.Warn($"RecognizeMove. Recognized: {recognizedPiece}, Real: {test.Piece}. Testcase {test.ImageKey}. Input {input}");
                    }
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

        private IScreenshot GetScreenshot(IQuantizer quantizer, IImage image, Point[] keypoints, double shift)
        {
            var shiftedKeypoints = ShiftKeypoints(keypoints, shift);
            quantizer.Keypoints = shiftedKeypoints;

            var quantized = quantizer.Quantize(image);
            var screenshot = new EmguScreenshot(quantized, DateTime.Now.Subtract(DateTime.MinValue));

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
        public IQuantizer Quantizer { get; }
        public int Shift { get; }

        public ExtractionComparisonInput(IExtractor extractor, IQuantizer quantizer, int shift)
        {
            Extractor = extractor;
            Quantizer = quantizer;
            Shift = shift;
        }

        public override string ToString()
        {
            return $"{{ Shift: {Shift} }}";
        }
    }

    public class ExtractionComparisonResult
    {
        public string MethodName { get; }
        public string ExtractorName { get; }
        public string QuantizerName { get; }
        public int Shift { get; }

        public int Total { get; set; }
        public int Recognized { get; set; }
        public double Accuracy { get; private set; }

        public ExtractionComparisonResult(ExtractionComparisonInput input, string methodName)
        {
            MethodName = methodName;
            ExtractorName = input.Extractor.GetType().Name;
            QuantizerName = input.Quantizer.GetType().Name;
            Shift = input.Shift;
        }

        public void Calculate()
        {
            Accuracy = (double)Recognized / Total;
        }
    }
}
