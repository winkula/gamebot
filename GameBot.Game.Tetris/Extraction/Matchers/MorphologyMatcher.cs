using System;
using System.Drawing;
using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Extensions;
using GameBot.Game.Tetris.Data;
using NLog;

namespace GameBot.Game.Tetris.Extraction.Matchers
{
    public class MorphologyMatcher : IMatcher
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public double GetProbability(IScreenshot screenshot, int x, int y)
        {
            if (screenshot == null) throw new ArgumentNullException(nameof(screenshot));
            if (x < 0 || x > 19) throw new ArgumentException("invalid x coordinate (out of the board)");
            if (y < 0 || y > 17) throw new ArgumentException("invalid y coordinate (out of the board)");

            var dest = screenshot.Image;
            var coordinates = Coordinates.BoardToTile(x, y);

            return GetBlockProbability(dest, coordinates);
        }

        public double GetProbability(IScreenshot screenshot, Piece piece)
        {
            if (screenshot == null) throw new ArgumentNullException(nameof(screenshot));
            if (piece == null) throw new ArgumentNullException(nameof(piece));
            if (piece.X < -4 || piece.X > 5) throw new ArgumentException($"piece has illegal x coordinate {piece.X}");
            if (piece.Y < -16 || piece.Y > 0) throw new ArgumentException($"piece has illegal y coordinate {piece.Y}");

            var dest = screenshot.Image;

            const double numBlocks = 4;
            double probabilitySum = 0.0;
            foreach (var body in piece.Shape.Body)
            {
                var coordinates = Coordinates.PieceToTile(piece.X + body.X, piece.Y + body.Y);
                var probability = GetBlockProbability(dest, coordinates);
                probabilitySum += probability;
            }

            return probabilitySum / numBlocks;
        }

        public double GetProbabilityNextPiece(IScreenshot screenshot, Tetrimino tetrimino)
        {
            if (screenshot == null) throw new ArgumentNullException(nameof(screenshot));

            var dest = screenshot.Image;

            const double numBlocks = 4;
            double probabilitySum = 0.0;

            var shape = Shape.Get(tetrimino);
            foreach (var point in shape.Body)
            {
                var coordinates = Coordinates.PieceToTilePreview(point);
                var probability = GetBlockProbability(dest, coordinates);

                probabilitySum += probability;
            }

            return probabilitySum / numBlocks;
        }

        private double GetBlockProbability(Mat image, Point tileCoordinates)
        {
            // probability that block is outside of the screen is always 0.0
            if (tileCoordinates.X < 0 || tileCoordinates.X >= GameBoyConstants.ScreenWidth / GameBoyConstants.TileSize) return 0.0;
            if (tileCoordinates.Y < 0 || tileCoordinates.Y >= GameBoyConstants.ScreenHeight / GameBoyConstants.TileSize) return 0.0;

            var roi = new Rectangle(GameBoyConstants.TileSize * tileCoordinates.X, GameBoyConstants.TileSize * tileCoordinates.Y, GameBoyConstants.TileSize, GameBoyConstants.TileSize);
            var imageRoi = new Mat((Mat)image, roi);

            var mean = CvInvoke.Mean(imageRoi);
            return ((255.0 - mean.V0) / 255.0).Clamp(0.0, 1.0);
        }
    }
}
