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

namespace GameBot.Test
{
    public class TestImageFactory
    {
        private static readonly ICalibrateableQuantizer _quantizer = new Quantizer(new AppSettingsConfig());

        private static readonly Point[][] _keypoints =
        {
            new [] { new Point(583,361), new Point(206,358), new Point(569,59), new Point(229,59) }, // series 00
            new [] { new Point(585,360), new Point(207,359), new Point(571,58), new Point(228,57) }, // series 01
            new [] { new Point(593,357), new Point(206,354), new Point(574,53), new Point(230,53) }, // series 02
            new [] { new Point(590,367), new Point(211,365), new Point(572,67), new Point(235,67) } // series 03
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
        
        public static IEnumerable TestCasesCurrentPiece
        {
            get
            {
                foreach (var data in _data)
                {
                    var imageKey = (string)data[0];
                    var imagePath = $"Images/test{imageKey}.jpg";
                    var keypoints = _keypoints[int.Parse(imageKey.Substring(0, 2))];
                    var currentPiece = (Piece) data[1];

                    if (currentPiece != null)
                    {
                        _quantizer.Calibrate(keypoints);

                        var image = new Mat(imagePath, LoadImageType.AnyColor);
                        var quantizedImage = _quantizer.Quantize(image);
                        var screenshot = new EmguScreenshot(quantizedImage, TimeSpan.Zero);
                        
                        yield return new TestCaseData(imageKey, screenshot, currentPiece);
                    }
                }
            }
        }
        
        public static IEnumerable TestCasesCurrentPieceNull
        {
            get
            {
                foreach (var data in _data)
                {
                    var imageKey = (string)data[0];
                    var imagePath = $"Images/test{imageKey}.jpg";
                    var keypoints = _keypoints[int.Parse(imageKey.Substring(0, 2))];
                    var currentPiece = (Piece)data[1];

                    if (currentPiece == null)
                    {
                        _quantizer.Calibrate(keypoints);

                        var image = new Mat(imagePath, LoadImageType.AnyColor);
                        var quantizedImage = _quantizer.Quantize(image);
                        var screenshot = new EmguScreenshot(quantizedImage, TimeSpan.Zero);

                        yield return new TestCaseData(imageKey, screenshot);
                    }
                }
            }
        }

        public static IEnumerable TestCasesSpawnedPiece
        {
            get
            {
                foreach (var data in _data)
                {
                    var imageKey = (string)data[0];
                    var imagePath = $"Images/test{imageKey}.jpg";
                    var keypoints = _keypoints[int.Parse(imageKey.Substring(0, 2))];
                    var currentPiece = (Piece)data[1];

                    if (currentPiece != null && currentPiece.IsUntouched)
                    {
                        _quantizer.Calibrate(keypoints);

                        var image = new Mat(imagePath, LoadImageType.AnyColor);
                        var quantizedImage = _quantizer.Quantize(image);
                        var screenshot = new EmguScreenshot(quantizedImage, TimeSpan.Zero);

                        yield return new TestCaseData(imageKey, screenshot, currentPiece);
                    }
                }
            }
        }

        public static IEnumerable TestCasesSpawnedPieceNull
        {
            get
            {
                foreach (var data in _data)
                {
                    var imageKey = (string)data[0];
                    var imagePath = $"Images/test{imageKey}.jpg";
                    var keypoints = _keypoints[int.Parse(imageKey.Substring(0, 2))];
                    var currentPiece = (Piece)data[1];

                    if (currentPiece == null || !currentPiece.IsUntouched)
                    {
                        _quantizer.Calibrate(keypoints);

                        var image = new Mat(imagePath, LoadImageType.AnyColor);
                        var quantizedImage = _quantizer.Quantize(image);
                        var screenshot = new EmguScreenshot(quantizedImage, TimeSpan.Zero);

                        yield return new TestCaseData(imageKey, screenshot);
                    }
                }
            }
        }

        public static IEnumerable TestCasesNextPiece
        {
            get
            {
                foreach (var data in _data)
                {
                    var imageKey = (string)data[0];
                    var imagePath = $"Images/test{imageKey}.jpg";
                    var keypoints = _keypoints[int.Parse(imageKey.Substring(0, 2))];
                    var nextPiece = (Tetromino?)data[2];

                    if (nextPiece.HasValue)
                    {
                        _quantizer.Calibrate(keypoints);

                        var image = new Mat(imagePath, LoadImageType.AnyColor);
                        var quantizedImage = _quantizer.Quantize(image);
                        var screenshot = new EmguScreenshot(quantizedImage, TimeSpan.Zero);

                        yield return new TestCaseData(imageKey, screenshot, nextPiece.Value);
                    }
                }
            }
        }
        
        public static IEnumerable TestCasesNextPieceNull
        {
            get
            {
                foreach (var data in _data)
                {
                    var imageKey = (string)data[0];
                    var imagePath = $"Images/test{imageKey}.jpg";
                    var keypoints = _keypoints[int.Parse(imageKey.Substring(0, 2))];
                    var nextPiece = (Tetromino?) data[2];

                    if (!nextPiece.HasValue)
                    {
                        _quantizer.Calibrate(keypoints);

                        var image = new Mat(imagePath, LoadImageType.AnyColor);
                        var quantizedImage = _quantizer.Quantize(image);
                        var screenshot = new EmguScreenshot(quantizedImage, TimeSpan.Zero);

                        yield return new TestCaseData(imageKey, screenshot);
                    }
                }
            }
        }
    }
}
