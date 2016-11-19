using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameBot.Core.Quantizers;

namespace GameBot.Test
{
    public class TestDataFactory
    {
        public class TestData
        {
            public string ImageKey { get; }
            public Mat Image { get; }
            private string ImagePath => $"Images/test{ImageKey}.jpg";
            public Point[] Keypoints { get; }
            public Piece Piece { get; }
            public Tetrimino? NextPiece { get; }
            public Move? Move { get; }
            public IScreenshot Screenshot { get; }

            public TestData(string imageKey, Piece currentPiece, Tetrimino? nextPiece, Move? move = null)
            {
                ImageKey = imageKey;
                Keypoints = _keypoints[int.Parse(imageKey.Substring(0, 2))];
                Piece = currentPiece;
                NextPiece = nextPiece;
                Move = move;

                _quantizer.Keypoints = Keypoints;

                Image = new Mat(ImagePath, LoadImageType.Grayscale);
                var quantizedImage = _quantizer.Quantize(Image);
                Screenshot = new EmguScreenshot(quantizedImage, DateTime.Now.Subtract(DateTime.MinValue));
            }
        }

        private static readonly IQuantizer _quantizer = new MorphologyQuantizer(TestHelper.GetFakeConfig().Object);

        private static readonly Point[][] _keypoints =
        {
            new [] { new Point(583,361), new Point(206,358), new Point(569,59), new Point(229,59) }, // series 00
            new [] { new Point(585,360), new Point(207,359), new Point(571,58), new Point(228,57) }, // series 01
            new [] { new Point(593,357), new Point(206,354), new Point(574,53), new Point(230,53) }, // series 02
            new [] { new Point(590,367), new Point(211,365), new Point(572,67), new Point(235,67) }, // series 03
            new [] { new Point(140,116), new Point(474,119), new Point(164,368), new Point(445,368) } // series 04
        };

        public static readonly IEnumerable<TestData> Data = new List<TestData>
        {
            new TestData("0000", new Piece(Tetrimino.T), Tetrimino.J, Move.Right),
            new TestData("0001", new Piece(Tetrimino.J), Tetrimino.S, Move.Left),
            new TestData("0002", new Piece(Tetrimino.S), Tetrimino.S, Move.RotateCounterclockwise),
            new TestData("0003", new Piece(Tetrimino.S), Tetrimino.O, Move.Rotate),
            new TestData("0004", new Piece(Tetrimino.O), Tetrimino.J, Move.Left),
            new TestData("0005", new Piece(Tetrimino.J), Tetrimino.T, Move.Right),
            new TestData("0006", new Piece(Tetrimino.L), Tetrimino.J, Move.Left),
            new TestData("0007", new Piece(Tetrimino.J), Tetrimino.L, Move.Rotate),
            new TestData("0008", new Piece(Tetrimino.L), Tetrimino.L, Move.Right),
            new TestData("0009", new Piece(Tetrimino.L), Tetrimino.T, Move.RotateCounterclockwise),
            new TestData("0010", new Piece(Tetrimino.T), Tetrimino.T, Move.Rotate),
            new TestData("0011", new Piece(Tetrimino.Z), Tetrimino.J, Move.Right),

            new TestData("0100", new Piece(Tetrimino.O), Tetrimino.I, Move.Right),
            new TestData("0101", new Piece(Tetrimino.I), Tetrimino.T, Move.Left),
            new TestData("0102", new Piece(Tetrimino.T), Tetrimino.O, Move.Left),
            new TestData("0103", new Piece(Tetrimino.T, 0, 3, -5), Tetrimino.O, Move.Rotate),
            new TestData("0104", new Piece(Tetrimino.T, 0, 3, -10), Tetrimino.O, Move.Right),
            new TestData("0105", new Piece(Tetrimino.O, 0, 4, -4), Tetrimino.J, Move.Left),
            new TestData("0106", new Piece(Tetrimino.O, 0, 4, -7), Tetrimino.J, Move.Left),
            new TestData("0107", new Piece(Tetrimino.J).Fall(2), null, Move.RotateCounterclockwise),

            new TestData("0200", new Piece(Tetrimino.T).Fall(2), Tetrimino.L, Move.Rotate),
            new TestData("0201", new Piece(Tetrimino.T, 0, -1, -9), Tetrimino.L, Move.RotateCounterclockwise),
            new TestData("0202", new Piece(Tetrimino.L, 0, -3, -4), Tetrimino.O, Move.Rotate),
            new TestData("0203", new Piece(Tetrimino.O).Fall(3), Tetrimino.J, Move.Right),
            new TestData("0204", new Piece(Tetrimino.O, 0, -2, -10), Tetrimino.J, Move.Left),
            new TestData("0205", new Piece(Tetrimino.J, 0, 4, -6), Tetrimino.O, Move.Left),
            new TestData("0206", new Piece(Tetrimino.J, 0, 4, -8), Tetrimino.O, Move.Rotate),
            new TestData("0207", new Piece(Tetrimino.O).Fall(2), Tetrimino.T, Move.Right),
            new TestData("0208", new Piece(Tetrimino.O, 0, 2, -5), Tetrimino.T, Move.Left),
            new TestData("0209", new Piece(Tetrimino.T, 0, 4, -4), Tetrimino.L, Move.Left),
            new TestData("0210", new Piece(Tetrimino.L, 0, 4, -4), Tetrimino.J, Move.RotateCounterclockwise),
            new TestData("0211", new Piece(Tetrimino.S).Fall(4), null, Move.Left),
            new TestData("0212", new Piece(Tetrimino.S).Fall(), null, Move.Right),

            new TestData("0300", null, null), // pause menu
            
            new TestData("0400", new Piece(Tetrimino.S), Tetrimino.S),
            new TestData("0401", new Piece(Tetrimino.O), Tetrimino.L),
            new TestData("0402", new Piece(Tetrimino.Z), Tetrimino.L),
            new TestData("0403", new Piece(Tetrimino.I), Tetrimino.Z),
            new TestData("0404", new Piece(Tetrimino.I), Tetrimino.J),
            new TestData("0405", new Piece(Tetrimino.O), Tetrimino.J),
            new TestData("0406", new Piece(Tetrimino.T), Tetrimino.I),
            new TestData("0407", new Piece(Tetrimino.S), Tetrimino.Z),
            new TestData("0408", new Piece(Tetrimino.O), Tetrimino.S),
            new TestData("0409", new Piece(Tetrimino.I), Tetrimino.O),
            new TestData("0410", new Piece(Tetrimino.J), Tetrimino.I),
            new TestData("0411", new Piece(Tetrimino.T), Tetrimino.T),
            new TestData("0412", new Piece(Tetrimino.S), Tetrimino.O),
            new TestData("0413", new Piece(Tetrimino.J), Tetrimino.S),
            new TestData("0414", new Piece(Tetrimino.I), Tetrimino.J),
            new TestData("0415", new Piece(Tetrimino.S), Tetrimino.T),
            new TestData("0416", null, Tetrimino.S),
            new TestData("0417", new Piece(Tetrimino.Z), Tetrimino.I)
        };

        public static IEnumerable TestCasesCurrentPiecePositives => Data
            .Where(x => x.Piece != null)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot, x.Piece));

        public static IEnumerable TestCasesCurrentPieceNegativesNull => Data
            .Where(x => x.Piece == null)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot));

        public static IEnumerable TestCasesCurrentPieceOriginNegatives => Data
            .Where(x => x.Piece == null || !x.Piece.IsOrigin)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot));

        public static IEnumerable TestCasesSpawnedPiecePositives => Data
            .Where(x => x.Piece != null)
            .Where(x => x.Piece.IsUntouched)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot, x.Piece));

        public static IEnumerable TestCasesSpawnedPieceNegativesNull => Data
            .Where(x => x.Piece == null || !x.Piece.IsUntouched)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot));

        public static IEnumerable TestCasesTouchedPieces => Data
            .Where(x => x.Piece != null)
            .Where(x => !x.Piece.IsUntouched)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot, x.Piece));

        /// <summary>
        /// Should be correctly recognized.
        /// </summary>
        public static IEnumerable TestCasesNextPiecePositives => Data
            .Where(x => x.NextPiece.HasValue)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot, x.NextPiece));

        /// <summary>
        /// Should be classified as false.
        /// Because no next piece is visible or the wrong piece is visible.
        /// </summary>
        public static IEnumerable TestCasesNextPieceNegativesNull => Data
            .Where(x => !x.NextPiece.HasValue)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot));

        public static IEnumerable TestCasesMovedPiece => Data
            .Where(x => x.Move.HasValue)
            .Select(x => new TestCaseData(x.ImageKey, x.Screenshot, x.Piece, x.Move));
    }
}
