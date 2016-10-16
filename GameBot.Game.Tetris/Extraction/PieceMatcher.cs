using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public class PieceMatcher
    {
        private const int _templateSize = 4 * GameBoyConstants.TileSize;
        private static readonly Mat _black = new Mat(new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight), DepthType.Cv8U, 1);

        private static readonly int[,] _templateIndexTable =
        {
           { 0, 0, 0, 0 },
           { 1, 2, 1, 2 },
           { 3, 4, 3, 4 },
           { 5, 6, 5, 6 },
           { 7, 8, 9, 10 },
           { 11, 12, 13, 14 },
           { 15, 16, 17, 18 }
        };

        private readonly Image<Gray, byte> _templates = new Image<Gray, byte>("Screenshots/templates.png");

        public PieceMatcher()
        {

        }

        private int GetTemplateIndex(Piece piece)
        {
            return _templateIndexTable[(int)piece.Tetromino, piece.Orientation];
        }

        private double GetError(double sum)
        {
            const double maxError = 4 * 8 * 8 * 255; // 4 blocks fully white
            return sum / maxError;
        }

        private double GetProbability(double sum)
        {
            return 1 - GetError(sum);
        }

        /// <summary>
        /// Gets the probability that a specific piece is visible on the screenshot.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="piece">The piece to match.</param>
        /// <returns>The probability.</returns>
        public double GetProbability(IScreenshot screenshot, Piece piece)
        {
            // we need some extra space over the screenshot, so that the template is not off the image
            // we add an extra one that we can make 1 pixel shifts to compensate errors in the camera calibration
            const int yTopPadding = GameBoyConstants.TileSize + 1;
            const int yBottomPadding = GameBoyConstants.TileSize + 1;

            // take upper left block in the 4 by 4 search window
            var tileCoordinates = Coordinates.PieceToTile(piece.X - 1, piece.Y + 2);
            
            // load screenshot
            // TODO: make faster!?
            var original = new Image<Gray, byte>(screenshot.Image.Bitmap);
            
            // get template and it's mask
            var templateIndex = GetTemplateIndex(piece);
            var template = new Image<Gray, byte>(_templateSize, _templateSize);
            var templateMask = new Image<Gray, byte>(_templateSize, _templateSize);
            _templates.ROI = new Rectangle(0, templateIndex * _templateSize, _templateSize, _templateSize);
            _templates.CopyTo(template);
            _templates.ROI = new Rectangle(_templateSize, templateIndex * _templateSize, _templateSize, _templateSize);
            _templates.CopyTo(templateMask);

            double bestProbability = 0;

            foreach (var shift in new[]
            {
                new Point(-1, -1), new Point(0, -1), new Point(1, -1),
                new Point(-1, 0), new Point(0, 0), new Point(1, 0),
                new Point(-1, 1), new Point(0, 1), new Point(1, 1)
            })
            {
                // create combined and reference image (that we can compare them later)
                var combined = new Image<Gray, byte>(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight + yTopPadding + yBottomPadding);
                var mainRoi = new Rectangle(0, yTopPadding, GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight);
                combined.ROI = mainRoi;
                original.CopyTo(combined);
                combined.ROI = Rectangle.Empty;
                var reference = combined.Clone();
                combined.ROI = mainRoi;

                // combine image with template
                var roiX = tileCoordinates.X * GameBoyConstants.TileSize + shift.X;
                var roiY = tileCoordinates.Y * GameBoyConstants.TileSize + yTopPadding + shift.Y;
                combined.ROI = new Rectangle(
                    roiX, roiY,
                    _templateSize, _templateSize);
                template.Copy(combined, templateMask);
                reference.ROI = combined.ROI;

                var difference = new Mat();
                CvInvoke.AbsDiff(reference, combined, difference);
                var sum = CvInvoke.Sum(difference);

                var newProbability = GetProbability(sum.V0);
                bestProbability = Math.Max(bestProbability, newProbability);

                //CvInvoke.Imshow("Combined", combined);
                //CvInvoke.Imshow("Ori", original);
                //CvInvoke.Imshow("diff", difference);
                //CvInvoke.WaitKey();
            }

            return bestProbability;
        }
    }
}
