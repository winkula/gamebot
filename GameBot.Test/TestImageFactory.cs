using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using GameBot.Core.Configuration;
using GameBot.Core.Data;
using GameBot.Engine.Physical.Quantizers;
using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Test
{
    public class TestImageFactory
    {
        public class TestData
        {
            public string ImageKey { get; }
            private string ImagePath => $"Images/test{ImageKey}.jpg";
            private Point[] Keypoints { get; }
            public Piece Piece { get; }
            public Tetromino? NextPiece { get; }
            public Move? Move { get; }
            public IScreenshot Screenshot { get; }

            public TestData(string imageKey, Piece currentPiece, Tetromino? nextPiece, Move? move = null)
            {
                ImageKey = imageKey;
                Keypoints = _keypoints[int.Parse(imageKey.Substring(0, 2))];
                Piece = currentPiece;
                NextPiece = nextPiece;
                Move = move;

                _quantizer.Calibrate(Keypoints);

                var image = new Mat(ImagePath, LoadImageType.AnyColor);
                var quantizedImage = _quantizer.Quantize(image);
                Screenshot = new EmguScreenshot(quantizedImage, TimeSpan.Zero);
            }
        }

        private static readonly ICalibrateableQuantizer _quantizer = new Quantizer(new AppSettingsConfig());

        private static readonly Point[][] _keypoints =
        {
            new [] { new Point(583,361), new Point(206,358), new Point(569,59), new Point(229,59) }, // series 00
            new [] { new Point(585,360), new Point(207,359), new Point(571,58), new Point(228,57) }, // series 01
            new [] { new Point(593,357), new Point(206,354), new Point(574,53), new Point(230,53) }, // series 02
            new [] { new Point(590,367), new Point(211,365), new Point(572,67), new Point(235,67) } // series 03
        };

        private static readonly IEnumerable<TestData> _data = new List<TestData>
        {
            new TestData("0000", new Piece(Tetromino.T), Tetromino.J, Move.Right),
            new TestData("0001", new Piece(Tetromino.J), Tetromino.S, Move.Left),
            new TestData("0002", new Piece(Tetromino.S), Tetromino.S, Move.RotateCounterclockwise),
            new TestData("0003", new Piece(Tetromino.S), Tetromino.O, Move.Rotate),
            new TestData("0004", new Piece(Tetromino.O), Tetromino.J, Move.Left),
            new TestData("0005", new Piece(Tetromino.J), Tetromino.T, Move.Right),
            new TestData("0006", new Piece(Tetromino.L), Tetromino.J, Move.Left),
            new TestData("0007", new Piece(Tetromino.J), Tetromino.L, Move.Rotate),
            new TestData("0008", new Piece(Tetromino.L), Tetromino.L, Move.Right),
            new TestData("0009", new Piece(Tetromino.L), Tetromino.T, Move.RotateCounterclockwise),
            new TestData("0010", new Piece(Tetromino.T), Tetromino.T, Move.Rotate),
            new TestData("0011", new Piece(Tetromino.Z), Tetromino.J, Move.Right),

            new TestData("0100", new Piece(Tetromino.O), Tetromino.I, Move.Right),
            new TestData("0101", new Piece(Tetromino.I), Tetromino.T, Move.Left),
            new TestData("0102", new Piece(Tetromino.T), Tetromino.O, Move.Left),
            new TestData("0103", new Piece(Tetromino.T, 0, 3, -5), Tetromino.O, Move.Rotate),
            new TestData("0104", new Piece(Tetromino.T, 0, 3, -10), Tetromino.O, Move.Right),
            new TestData("0105", new Piece(Tetromino.O, 0, 4, -4), Tetromino.J, Move.Left),
            new TestData("0106", new Piece(Tetromino.O, 0, 4, -7), Tetromino.J, Move.Left),
            new TestData("0107", new Piece(Tetromino.J).Fall(2), null, Move.RotateCounterclockwise),

            new TestData("0200", new Piece(Tetromino.T).Fall(2), Tetromino.L, Move.Rotate),
            new TestData("0201", new Piece(Tetromino.T, 0, -1, -9), Tetromino.L, Move.RotateCounterclockwise),
            new TestData("0202", new Piece(Tetromino.L, 0, -3, -4), Tetromino.O, Move.Rotate),
            new TestData("0203", new Piece(Tetromino.O).Fall(3), Tetromino.J, Move.Right),
            new TestData("0204", new Piece(Tetromino.O, 0, -2, -10), Tetromino.J, Move.Left),
            new TestData("0205", new Piece(Tetromino.J, 0, 4, -6), Tetromino.O, Move.Left),
            new TestData("0206", new Piece(Tetromino.J, 0, 4, -8), Tetromino.O, Move.Rotate),
            new TestData("0207", new Piece(Tetromino.O).Fall(2), Tetromino.T, Move.Right),
            new TestData("0208", new Piece(Tetromino.O, 0, 2, -5), Tetromino.T, Move.Left),
            new TestData("0209", new Piece(Tetromino.T, 0, 4, -4), Tetromino.L, Move.Left),
            new TestData("0210", new Piece(Tetromino.L, 0, 4, -4), Tetromino.J, Move.RotateCounterclockwise),
            new TestData("0211", new Piece(Tetromino.S).Fall(4), null, Move.Left),
            new TestData("0212", new Piece(Tetromino.S).Fall(), null, Move.Right),

            new TestData("0300", null, null) // pause menu
        };
        
        public static IEnumerable TestCasesCurrentPiecePositives => _data
            .Where(x => x.Piece != null)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot, x.Piece));

        public static IEnumerable TestCasesCurrentPieceNegativesNull => _data
            .Where(x => x.Piece == null)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot));

        public static IEnumerable TestCasesSpawnedPiecePositives => _data
            .Where(x => x.Piece != null)
            .Where(x => x.Piece.IsUntouched)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot, x.Piece));

        public static IEnumerable TestCasesSpawnedPieceNegativesNull => _data
            .Where(x => x.Piece == null || !x.Piece.IsUntouched)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot));

        public static IEnumerable TestCasesTouchedPieces => _data
            .Where(x => x.Piece != null)
            .Where(x => !x.Piece.IsUntouched)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot, x.Piece));

        /// <summary>
        /// Should be correctly recognized.
        /// </summary>
        public static IEnumerable TestCasesNextPiecePositives => _data
            .Where(x => x.NextPiece.HasValue)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot, x.NextPiece));

        /// <summary>
        /// Should be classified as false.
        /// Because no next piece is visible or the wrong piece is visible.
        /// </summary>
        public static IEnumerable TestCasesNextPieceNegativesNull => _data
            .Where(x => !x.NextPiece.HasValue)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot));
       
        public static IEnumerable TestCasesMovedPiece => _data
            .Where(x => x.Move.HasValue)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot, x.Piece, x.Move));
    }
}
