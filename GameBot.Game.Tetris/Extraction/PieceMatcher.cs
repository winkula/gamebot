using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private const int TemplateSize = 4 * GameBoyConstants.TileSize;
        private static readonly Mat Black = new Mat(new Size(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight), DepthType.Cv8U, 1);

        private static readonly int[,] TemplateIndexTable =
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
            return TemplateIndexTable[(int)piece.Tetromino, piece.Orientation];
        }

        /// <summary>
        /// Gets the probability that a specific piece is visible on the screenshot.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="piece">The piece to match.</param>
        /// <returns>The probability.</returns>
        public double GetProbability(IScreenshot screenshot, Piece piece)
        {
            // take upper left block in the 4 by 4 search window
            var tileCoordinates = Coordinates.PieceToTile(piece.X - 1, piece.Y + 2);

            // TODO: make faster!
            var original = new Image<Gray, byte>(screenshot.Image.Bitmap);

            var combined = new Image<Gray, byte>(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight + GameBoyConstants.TileSize);
            var roi = new Rectangle(0, GameBoyConstants.TileSize, GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight);
            combined.ROI = roi;
            original.CopyTo(combined);
            combined.ROI = Rectangle.Empty;

            original = combined.Clone();
            combined.ROI = roi;

            var templateIndex = GetTemplateIndex(piece);

            var template = new Image<Gray, byte>(TemplateSize, TemplateSize);
            var templateMask = new Image<Gray, byte>(TemplateSize, TemplateSize);

            _templates.ROI = new Rectangle(0, templateIndex * TemplateSize, TemplateSize, TemplateSize);
            _templates.CopyTo(template);
            _templates.ROI = new Rectangle(TemplateSize, templateIndex * TemplateSize, TemplateSize, TemplateSize);
            _templates.CopyTo(templateMask);

            combined.ROI = new Rectangle(
                tileCoordinates.X * GameBoyConstants.TileSize,
                tileCoordinates.Y * GameBoyConstants.TileSize + GameBoyConstants.TileSize,
                TemplateSize, TemplateSize);

            template.Copy(combined, templateMask);
            
            original.ROI = combined.ROI;

            var difference = new Mat();
            CvInvoke.AbsDiff(original, combined, difference);
            var sum = CvInvoke.Sum(difference);

            //CvInvoke.Imshow("Combined", combined);
            //CvInvoke.Imshow("Ori", original);
            //CvInvoke.Imshow("diff", difference);
            //CvInvoke.WaitKey();

            double maxError = 4 * 8 * 8 * 255;
            double relativeError = sum.V0 / maxError;
            return 1 - relativeError;
        }
    }
}
