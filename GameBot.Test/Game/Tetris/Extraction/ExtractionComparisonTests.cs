using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Exceptions;
using GameBot.Core.Extensions;
using GameBot.Core.Quantizers;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Extractors;
using GameBot.Test.Extensions;
using Moq;
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

        private Mock<IConfig> _configMock;
        private Random _random;
        private IQuantizer _quantizer;
        private IQuantizer _quantizerMorphology;

        [TestFixtureSetUp]
        public void Init()
        {
            _configMock = TestHelper.GetFakeConfig();
            _configMock.ConfigValue("Game.Tetris.Extractor.ThresholdNextPiece", 0.6);
            _configMock.ConfigValue("Game.Tetris.Extractor.ThresholdCurrentPiece", 0.6);
            _configMock.ConfigValue("Game.Tetris.Extractor.ThresholdMovedPiece", 0.5);

            _random = new Random(123);
            _quantizer = new Quantizer(_configMock.Object);
            _quantizerMorphology = new MorphologyQuantizer(_configMock.Object);
        }

        [Test]
        public void Compare()
        {
            var extractors = new IExtractor[] { /*new BlockBasedExtractor(), new PieceBasedExtractor(),*/ new MorphologyExtractor(_configMock.Object) };
            var quantizers = new IQuantizer[] { /*_quantizer,*/ _quantizerMorphology };
            var shifts = new[] { 0, 1, 2, 4, 8 };
            var noises = new[] { 0.0, 0.25 };

            var results = new List<ExtractionComparisonResult>();
            foreach (var quantizer in quantizers)
            {
                foreach (var extractor in extractors)
                {
                    foreach (var shift in shifts)
                    {
                        foreach (var noise in noises)
                        {
                            var input = new ExtractionComparisonInput(extractor, quantizer, shift, noise);

                            // run tests...
                            AddResults(results, RecognizeNextPiece(input));
                            AddResults(results, RecognizeCurrentPieceUnknown(input));
                            AddResults(results, RecognizeCurrentPieceKnown(input));
                            AddResults(results, RecognizeMove(input));
                        }
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

            var positiveTests = TestDataFactory.Data
                .Where(x => x.NextPiece.HasValue)
                .Take(46)
                .ToList();

            for (int i = 0; i < _multiplySamplesBy; i++)
            {
                foreach (var test in positiveTests)
                {
                    var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, input);

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
            
            var negativeTests = TestDataFactory.Data
                .Where(x => !x.NextPiece.HasValue)
                .ToList();

            for (int i = 0; i < _multiplySamplesBy; i++)
            {
                foreach (var test in negativeTests)
                {
                    var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, input);

                    var nextPiece = input.Extractor.ExtractNextPiece(screenshot);

                    results.Total++;
                    if (nextPiece == null)
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

            var positiveTests = TestDataFactory.Data
                .Where(x => x.Piece != null)
                .Where(x => x.Piece.IsUntouched)
                .Take(34)
                .ToList();

            for (int i = 0; i < _multiplySamplesBy; i++)
            {
                foreach (var test in positiveTests)
                {
                    var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, input);

                    var currentPiece = input.Extractor.ExtractCurrentPiece(screenshot, null, test.Piece.FallHeight);

                    results.Total++;
                    if (currentPiece != null && currentPiece.Tetrimino == test.Piece.Tetrimino)
                    {
                        results.Recognized++;
                    }
                }
            }

            var negativeTests = TestDataFactory.Data
                .Where(x =>
                    x.Piece == null ||
                    x.Piece.FallHeight >= 3 ||
                    (!x.Piece.IsUntouched && x.Piece.FallHeight >= 3))
                .ToList();

            for (int i = 0; i < _multiplySamplesBy; i++)
            {
                foreach (var test in negativeTests)
                {
                    var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, input);

                    var currentPiece = input.Extractor.ExtractCurrentPiece(screenshot, null, 0);

                    results.Total++;
                    if (currentPiece == null)
                    {
                        results.Recognized++;
                    }
                }
            }

            return results;
        }

        private ExtractionComparisonResult RecognizeCurrentPieceKnown(ExtractionComparisonInput input)
        {
            var results = new ExtractionComparisonResult(input, nameof(RecognizeCurrentPieceKnown));

            var positiveTests = TestDataFactory.Data
                .Where(x => x.Piece != null)
                .Where(x => x.Piece.IsUntouched)
                .Take(34)
                .ToList();

            for (int i = 0; i < _multiplySamplesBy; i++)
            {
                foreach (var test in positiveTests)
                {
                    var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, input);

                    var currentPiece = input.Extractor.ExtractCurrentPiece(screenshot, test.Piece.Tetrimino,
                        test.Piece.FallHeight);

                    results.Total++;
                    if (currentPiece != null && currentPiece.Tetrimino == test.Piece.Tetrimino)
                    {
                        results.Recognized++;
                    }
                }
            }

            var negativeTests = TestDataFactory.Data
                .Where(x =>
                    x.Piece == null ||
                    x.Piece.FallHeight >= 3 ||
                    (!x.Piece.IsUntouched && x.Piece.FallHeight >= 3))
                .ToList();

            for (int i = 0; i < _multiplySamplesBy; i++)
            {
                foreach (var test in negativeTests)
                {
                    var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, input);

                    var currentPiece = input.Extractor.ExtractCurrentPiece(screenshot, null, 0);

                    results.Total++;
                    if (currentPiece == null)
                    {
                        results.Recognized++;
                    }
                }
            }

            return results;
        }

        private ExtractionComparisonResult RecognizeMove(ExtractionComparisonInput input)
        {
            var results = new ExtractionComparisonResult(input, nameof(RecognizeMove));

            var tests = TestDataFactory.Data
                .Where(x => x.Piece != null)
                .ToList();

            for (int i = 0; i < _multiplySamplesBy; i++)
            {
                foreach (var test in tests)
                {
                    var screenshot = GetScreenshot(input.Quantizer, test.Image, test.Keypoints, input);

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
                }
            }

            return results;
        }

        private Move? GetValidMove(TestDataFactory.TestData test)
        {
            var validMoves = (test.Piece.Tetrimino == Tetrimino.O ?
                new[] { Move.Left, Move.Right } :
                new[] { Move.Left, Move.Right, Move.Rotate, Move.RotateCounterclockwise })
            .OrderBy(x => Guid.NewGuid())
            .ToArray();

            var gamestate = new GameState(new Piece(test.Piece), test.NextPiece);
            foreach (var move in validMoves)
            {
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

        private IScreenshot GetScreenshot(IQuantizer quantizer, Mat image, Point[] keypoints, ExtractionComparisonInput input)
        {
            var shiftedKeypoints = ShiftKeypoints(keypoints, input.Shift);
            quantizer.Keypoints = shiftedKeypoints;
            
            var noisedImage = image.AddNoise(input.Noise);
            var quantized = quantizer.Quantize(noisedImage);
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
        public double Noise { get; }

        public ExtractionComparisonInput(IExtractor extractor, IQuantizer quantizer, int shift, double noise)
        {
            Extractor = extractor;
            Quantizer = quantizer;
            Shift = shift;
            Noise = noise;
        }

        public override string ToString()
        {
            return $"{{ Shift: {Shift}, Noise: {Noise} }}";
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
