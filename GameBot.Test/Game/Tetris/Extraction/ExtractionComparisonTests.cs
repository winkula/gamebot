using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Configuration;
using GameBot.Core.Data;
using GameBot.Engine.Physical.Quantizers;
using GameBot.Game.Tetris.Extraction.Extractors;
using NLog;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class ExtractionComparisonTests
    {
        private static readonly Logger _logger = LogManager.GetLogger("Tests");

        private Random _random;
        private ICalibrateableQuantizer _quantizer;

        [TestFixtureSetUp]
        public void Init()
        {
            _random = new Random(123);
            _quantizer = new Quantizer(new AppSettingsConfig());
        }

        [Test]
        public void Compare()
        {
            var implementations = new IExtractor[] { /*new BlockBasedExtractor(),*/ new PieceBasedExtractor() };
            var shifts = new[] { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 10.0 };

            var sb = new StringBuilder();
            sb.AppendLine("Implementation;Method;Shift;Total;Positive;Accuracy");

            foreach (var implementation in implementations)
            {
                foreach (var shift in shifts)
                {
                    // run tests...
                    var results1 = RecognizeNextPiece(implementation, shift);
                    sb.AppendLine($"{implementation.GetType().Name};RecognizeNextPiece;{shift:F};{results1.Total};{results1.Recognized};{100.0 * results1.Recognized / results1.Total:F}");

                    var results2 = RecognizeCurrentPiece(implementation, shift);
                    sb.AppendLine($"{implementation.GetType().Name};RecognizeCurrentPiece;{shift:F};{results2.Total};{results2.Recognized};{100.0 * results2.Recognized / results2.Total:F}");

                    var results3 = RecognizeMove(implementation, shift);
                    sb.AppendLine($"{implementation.GetType().Name};RecognizeMove;{shift:F};{results3.Total};{results3.Recognized};{100.0 * results3.Recognized / results3.Total:F}");
                }
            }

            _logger.Info(sb.ToString());
        }

        private ExtractionComparisonResults RecognizeNextPiece(IExtractor extractor, double shift)
        {
            var results = new ExtractionComparisonResults();

            var tests = ImageTestCaseFactory.Data
                .Where(x => x.NextPiece.HasValue)
                .ToList();

            foreach (var test in tests)
            {
                var screenshot = GetScreenshot(test.Image, test.Keypoints, shift);

                var nextPiece = extractor.ExtractNextPiece(screenshot);

                results.Total++;
                if (test.NextPiece == nextPiece)
                {
                    results.Recognized++;
                }
            }

            return results;
        }

        private ExtractionComparisonResults RecognizeCurrentPiece(IExtractor extractor, double shift)
        {
            var results = new ExtractionComparisonResults();

            var tests = ImageTestCaseFactory.Data
                .Where(x => x.Piece != null)
                .Where(x => x.Piece.IsUntouched)
                .ToList();

            foreach (var test in tests)
            {
                var screenshot = GetScreenshot(test.Image, test.Keypoints, shift);
                
                var currentPiece = extractor.ExtractCurrentPiece(screenshot);

                results.Total++;
                if (test.Piece.Tetrimino == currentPiece)
                {
                    results.Recognized++;
                }
            }

            return results;
        }

        private ExtractionComparisonResults RecognizeMove(IExtractor extractor, double shift)
        {
            var results = new ExtractionComparisonResults();

            var tests = ImageTestCaseFactory.Data
                .Where(x => x.Piece != null)
                .ToList();

            foreach (var test in tests)
            {
                var screenshot = GetScreenshot(test.Image, test.Keypoints, shift);
                /*
                var nextPiece = extractor.ExtractNextPiece(screenshot);

                results.Total++;
                if (test.NextPiece == nextPiece)
                {
                    results.Recognized++;
                }*/
            }

            return results;
        }

        private IScreenshot GetScreenshot(IImage image, Point[] keypoints, double shift)
        {
            var shiftedKeypoints = ShiftKeypoints(keypoints, shift);
            _quantizer.Calibrate(shiftedKeypoints);

            var quantized = _quantizer.Quantize(image);
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

    public class ExtractionComparisonResults
    {
        public int Total { get; set; }
        public int Recognized { get; set; }
    }
}
