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

        private Random _random;

        [TestFixtureSetUp]
        public void Init()
        {
            _random = new Random(123);
        }

        [Test]
        public void TestDataAll()
        {
            IList<Tuple<int, double>> shiftNoises = new List<Tuple<int, double>>();
            shiftNoises.Add(new Tuple<int, double>(0, 0.0));
            shiftNoises.Add(new Tuple<int, double>(1, 0.0));
            shiftNoises.Add(new Tuple<int, double>(2, 0.0));
            shiftNoises.Add(new Tuple<int, double>(4, 0.0));
            shiftNoises.Add(new Tuple<int, double>(8, 0.0));
            shiftNoises.Add(new Tuple<int, double>(0, 0.2));
            shiftNoises.Add(new Tuple<int, double>(0, 0.4));
            shiftNoises.Add(new Tuple<int, double>(0, 0.6));
            shiftNoises.Add(new Tuple<int, double>(0, 0.8));
            
            var thresholds = Enumerable.Range(0, 11).Select(x => x / 10.0).ToList();

            var results = new List<ExtractionComparisonResult>();
            foreach (var shiftNoise in shiftNoises)
            {
                foreach (var threshold in thresholds)
                {
                    var shift = shiftNoise.Item1;
                    var noise = shiftNoise.Item2;

                    var candidate = GetCandidate(threshold, noise);

                    var input = new ExtractionComparisonInput(candidate.Item1, candidate.Item2, candidate.Item3, shift, noise, threshold);

                    // run tests...
                    AddResults(results, RecognizeNextPiece(input));
                    AddResults(results, RecognizeCurrentPieceUnknown(input));
                    AddResults(results, RecognizeCurrentPieceKnown(input));
                    AddResults(results, RecognizeMove(input));
                }
            }

            _loggerDetails.Info("Candidate;Extractor;Method;Threshold;Noise;Shift;PositivesTotal;PositivesOk;NegativesTotal;NegativesOk");
            foreach (var result in results)
            {
                _loggerDetails.Info($"{result.CandidateName};" +
                                    $"{result.ExtractorName};" +
                                    $"{result.MethodName};" +
                                    $"{result.Threshold};" +
                                    $"{result.Noise};" +
                                    $"{result.Shift};" +
                                    $"{result.PositivesTotal};" +
                                    $"{result.PositivesOk};" +
                                    $"{result.NegativesTotal};" +
                                    $"{result.NegativesOk}");
            }
        }

        private Tuple<string, IExtractor, IQuantizer> GetCandidate(double threshold, double noise)
        {
            var configMock = TestHelper.GetFakeConfig();

            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdNextPiece", threshold);
            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdCurrentPiece", threshold);
            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdMovedPiece", threshold);

            configMock.ConfigValue("Robot.Quantizer.Threshold.Constant", 13);
            configMock.ConfigValue("Robot.Quantizer.Threshold.BlockSize", 17);
            configMock.ConfigValue("Robot.Quantizer.NoiseLevel", noise);

            //var simpleQuantizer = new Quantizer(configMock.Object);
            var morphologyQuantizer = new MorphologyQuantizer(configMock.Object);

            var naiveBlockExtractor = new BlockBasedExtractor(configMock.Object);
            return new Tuple<string, IExtractor, IQuantizer>("NaiveBlockExtractor", naiveBlockExtractor, morphologyQuantizer);

            //var templateExtractor = new PieceBasedExtractor(configMock.Object);
            //return new Tuple<string, IExtractor, IQuantizer>("TemplatePieceExtractor", templateExtractor, simpleQuantizer);

            //var morphologyExtractor = new MorphologyExtractor(configMock.Object);
            //return new Tuple<string, IExtractor, IQuantizer>("MorphologyBlockExtractor", morphologyExtractor, morphologyQuantizer);
        }

        private IEnumerable<Tuple<string, IExtractor, IQuantizer>> GetCandidates(double noise)
        {
            var configMock = TestHelper.GetFakeConfig();

            configMock.ConfigValue("Robot.Quantizer.Threshold.Constant", 13);
            configMock.ConfigValue("Robot.Quantizer.Threshold.BlockSize", 17);
            configMock.ConfigValue("Robot.Quantizer.NoiseLevel", noise);

            var simpleQuantizer = new Quantizer(configMock.Object);
            var morphologyQuantizer = new MorphologyQuantizer(configMock.Object);

            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdNextPiece", 0.5);
            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdCurrentPiece", 0.5);
            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdMovedPiece", 0.5);
            var naiveBlockExtractor = new BlockBasedExtractor(configMock.Object);
            yield return new Tuple<string, IExtractor, IQuantizer>("NaiveBlockExtractor", naiveBlockExtractor, morphologyQuantizer);

            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdNextPiece", 0.2);
            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdCurrentPiece", 0.5);
            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdMovedPiece", 0.5);
            var templateExtractor = new PieceBasedExtractor(configMock.Object);
            yield return new Tuple<string, IExtractor, IQuantizer>("TemplatePieceExtractor", templateExtractor, simpleQuantizer);

            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdNextPiece", 0.6);
            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdCurrentPiece", 0.6);
            configMock.ConfigValue("Game.Tetris.Extractor.ThresholdMovedPiece", 0.5);
            var morphologyExtractor = new MorphologyExtractor(configMock.Object);
            yield return new Tuple<string, IExtractor, IQuantizer>("MorphologyBlockExtractor", morphologyExtractor, morphologyQuantizer);
        }

        [Test]
        public void StatsOfOne()
        {
            const double noise = 0.6;
            var thresholds = Enumerable.Range(0, 11).Select(x => x / 10.0).ToList();

            var results = new List<ExtractionComparisonResult>();

            foreach (var threshold in thresholds)
            {
                var candidate = GetCandidate(threshold, noise);

                var input = new ExtractionComparisonInput(candidate.Item1, candidate.Item2, candidate.Item3, 0, noise, threshold);

                // run tests...
                AddResults(results, RecognizeNextPiece(input));
                AddResults(results, RecognizeCurrentPieceUnknown(input));
                AddResults(results, RecognizeCurrentPieceKnown(input));
                AddResults(results, RecognizeMove(input));
            }

            _loggerDetails.Info("Threshold;NP_pos;NP_neg;CPU_pos;CPU_neg;CPK_pos;CPK_neg;M");
            foreach (var grouping in results.GroupBy(x => x.Threshold).ToList())
            {
                _loggerDetails.Info($"{grouping.Key};" +
                                    $"{grouping.Where(x => x.MethodName == "RecognizeNextPiece").Select(x => x.PositivesAccuracy).Average():F};" +
                                    $"{grouping.Where(x => x.MethodName == "RecognizeNextPiece").Select(x => x.NegativesAccuracy).Average():F};" +
                                    $"{grouping.Where(x => x.MethodName == "RecognizeCurrentPieceUnknown").Select(x => x.PositivesAccuracy).Average():F};" +
                                    $"{grouping.Where(x => x.MethodName == "RecognizeCurrentPieceUnknown").Select(x => x.NegativesAccuracy).Average():F};" +
                                    $"{grouping.Where(x => x.MethodName == "RecognizeCurrentPieceKnown").Select(x => x.PositivesAccuracy).Average():F};" +
                                    $"{grouping.Where(x => x.MethodName == "RecognizeCurrentPieceKnown").Select(x => x.NegativesAccuracy).Average():F};" +
                                    $"{grouping.Where(x => x.MethodName == "RecognizeMove").Select(x => x.OverallAccuracy).Average():F}");
            }
        }

        [Test]
        public void CompareAllByNoiseOrShift()
        {
            /*
            var shifts = new[] { 0, 1, 2, 4, 8 };
            var noises = new[] { 0.0 };
            */

            var shifts = new[] { 0 };
            var noises = new[] { 0.0, 0.2, 0.4, 0.6, 0.8 };


            var results = new List<ExtractionComparisonResult>();

            foreach (var shift in shifts)
            {
                foreach (var noise in noises)
                {
                    foreach (var candidate in GetCandidates(noise))
                    {
                        var input = new ExtractionComparisonInput(candidate.Item1, candidate.Item2, candidate.Item3, shift, noise, 1);

                        // run tests...
                        AddResults(results, RecognizeNextPiece(input));
                        AddResults(results, RecognizeCurrentPieceUnknown(input));
                        AddResults(results, RecognizeCurrentPieceKnown(input));
                        AddResults(results, RecognizeMove(input));
                    }
                }
            }

            if (shifts.Length > 1)
            {
                _loggerComparison.Info("Shift;NaiveBlockExtractor;TemplatePieceExtractor;MorphologyBlockExtractor");
                foreach (var grouping in results.GroupBy(x => x.Shift).ToList())
                {
                    _loggerComparison.Info($"{grouping.Key};" +
                                           $"{grouping.Where(x => x.CandidateName == "NaiveBlockExtractor").Select(x => x.OverallAccuracy).Average():F};" +
                                           $"{grouping.Where(x => x.CandidateName == "TemplatePieceExtractor").Select(x => x.OverallAccuracy).Average():F};" +
                                           $"{grouping.Where(x => x.CandidateName == "MorphologyBlockExtractor").Select(x => x.OverallAccuracy).Average():F}");
                }
            }

            if (noises.Length > 1)
            {
                _loggerComparison.Info("Noise;NaiveBlockExtractor;TemplatePieceExtractor;MorphologyBlockExtractor");
                foreach (var grouping in results.GroupBy(x => x.Noise).ToList())
                {
                    _loggerComparison.Info($"{grouping.Key};" +
                                           $"{grouping.Where(x => x.CandidateName == "NaiveBlockExtractor").Select(x => x.OverallAccuracy).Average():F};" +
                                           $"{grouping.Where(x => x.CandidateName == "TemplatePieceExtractor").Select(x => x.OverallAccuracy).Average():F};" +
                                           $"{grouping.Where(x => x.CandidateName == "MorphologyBlockExtractor").Select(x => x.OverallAccuracy).Average():F}");
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

                    results.PositivesTotal++;
                    if (test.NextPiece == nextPiece)
                    {
                        results.PositivesOk++;
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

                    results.NegativesTotal++;
                    if (nextPiece == null)
                    {
                        results.NegativesOk++;
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

                    results.PositivesTotal++;
                    if (currentPiece != null && currentPiece.Tetrimino == test.Piece.Tetrimino)
                    {
                        results.PositivesOk++;
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

                    results.NegativesTotal++;
                    if (currentPiece == null)
                    {
                        results.NegativesOk++;
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

                    results.PositivesTotal++;
                    if (currentPiece != null && currentPiece.Tetrimino == test.Piece.Tetrimino)
                    {
                        results.PositivesOk++;
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

                    results.NegativesTotal++;
                    if (currentPiece == null)
                    {
                        results.NegativesOk++;
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

                    results.PositivesTotal++;
                    if (recognizedPiece != null && recognizedPiece.Equals(test.Piece) && !moved)
                    {
                        results.PositivesOk++;
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
        public string Name { get; set; }
        public IExtractor Extractor { get; }
        public IQuantizer Quantizer { get; }
        public int Shift { get; }
        public double Noise { get; }
        public double Threshold { get; }

        public ExtractionComparisonInput(string name, IExtractor extractor, IQuantizer quantizer, int shift, double noise, double threshold)
        {
            Name = name;
            Extractor = extractor;
            Quantizer = quantizer;
            Shift = shift;
            Noise = noise;
            Threshold = threshold;
        }

        public override string ToString()
        {
            return $"{{ Shift: {Shift}, Noise: {Noise} }}";
        }
    }

    public class ExtractionComparisonResult
    {
        public string CandidateName { get; }
        public string MethodName { get; }
        public string ExtractorName { get; }
        public string QuantizerName { get; }
        public int Shift { get; }
        public double Noise { get; }
        public double Threshold { get; }

        public int PositivesTotal { get; set; }
        public int PositivesOk { get; set; }
        public double PositivesAccuracy { get; set; }

        public int NegativesTotal { get; set; }
        public int NegativesOk { get; set; }
        public double NegativesAccuracy { get; set; }

        public double OverallAccuracy { get; set; }

        public ExtractionComparisonResult(ExtractionComparisonInput input, string methodName)
        {
            CandidateName = input.Name;
            MethodName = methodName;
            ExtractorName = input.Extractor.GetType().Name;
            QuantizerName = input.Quantizer.GetType().Name;
            Shift = input.Shift;
            Noise = input.Noise;
            Threshold = input.Threshold;
        }

        public void Calculate()
        {
            PositivesAccuracy = (double)PositivesOk / PositivesTotal;
            NegativesAccuracy = (double)NegativesOk / NegativesTotal;
            OverallAccuracy = (double)(PositivesOk + NegativesOk) / (PositivesTotal + NegativesTotal);
        }
    }
}
