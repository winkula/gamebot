using System;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
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

        private readonly Mat _kernel;

        private IScreenshot _cachedScreenshot;
        private Image<Gray, byte> _chachedImage;

        public MorphologyMatcher()
        {
            // TODO: use a diagonal cross instad of a square?
            _kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(7, 7), new Point(-1, -1));
        }

        public double GetProbability(IScreenshot screenshot, Piece piece)
        {
            if (screenshot == null) throw new ArgumentNullException(nameof(screenshot));
            if (piece == null) throw new ArgumentNullException(nameof(piece));
            if (piece.X < -4 || piece.X > 5) throw new ArgumentException($"piece has illegal x coordinate {piece.X}");
            if (piece.Y < -16 || piece.Y > 0) throw new ArgumentException($"piece has illegal y coordinate {piece.Y}");

            var dest = MorphologyEx(screenshot);

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

            var dest = MorphologyEx(screenshot);

            const double numBlocks = 7;
            double probabilitySum = 0.0;

            var shape = Shape.Get(tetrimino);
            foreach (var point in shape.Body)
            {
                var coordinates = Coordinates.PieceToTilePreview(point);
                var probability = GetBlockProbability(dest, coordinates);

                probabilitySum += probability;

                /*
                if (shape.Body.Contains(point))
                {
                    // here must be a block
                    probabilitySum += probability;
                }
                else
                {
                    // here must be free tile
                    probabilitySum += (1 - probability);
                }*/
            }

            return probabilitySum / 4;

            /*
            if (screenshot == null) throw new ArgumentNullException(nameof(screenshot));

            var dest = MorphologyEx(screenshot);

            int blocks = 0;

            var shape = Shape.Get(tetrimino);
            foreach (var point in new[] { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(-1, -1), new Point(0, -1), new Point(1, -1) })
            {
                var coordinates = Coordinates.PieceToTilePreview(point);
                var isBlock = IsBlock(dest, coordinates);
                var shouldBeBlock = shape.Body.Contains(point);

                if (!(isBlock ^ shouldBeBlock))
                {
                    blocks++;
                }
            }

            // 3 blocks out of 7 corresponds to probability 0
            // 7 out of 7 blocks corresponds to probability 1
            var normedProbability = (blocks - 3) / 4.0;
            return normedProbability.Clamp(0.0, 1.0);
            */
        }

        private Image<Gray, byte> MorphologyEx(IScreenshot screenshot)
        {
            if (_cachedScreenshot != null && _cachedScreenshot.Equals(screenshot))
            {
                // we have already used this screenshot, take image from cache
                return _chachedImage;
            }
            
            var dst = new Image<Gray, byte>(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight);
            CvInvoke.MorphologyEx(screenshot.Image, dst, MorphOp.Open, _kernel, new Point(-1, -1), 1, BorderType.Replicate, new MCvScalar(-1));

            // cache image
            _cachedScreenshot = screenshot;
            _chachedImage = dst;

            return dst;
        }
        
        private double GetBlockProbability(Image<Gray, byte> image, Point tileCoordinates)
        {
            if (tileCoordinates.Y < 0) return 0.0; // still board, but not visible on screenshot
            if (tileCoordinates.X < 0 || tileCoordinates.X > 19) throw new ArgumentException("invalid x coordinate");
            if (tileCoordinates.Y > 17) throw new ArgumentException("invalid y coordinate");
            
            image.ROI = new Rectangle(GameBoyConstants.TileSize * tileCoordinates.X, GameBoyConstants.TileSize * tileCoordinates.Y, GameBoyConstants.TileSize, GameBoyConstants.TileSize);
            var mean = CvInvoke.Mean(image);
            return ((255.0 - mean.V0) / 255.0).Clamp(0.0, 1.0);
        }

        private bool IsBlock(Image<Gray, byte> image, Point tileCoordinates)
        {
            var probability = GetBlockProbability(image, tileCoordinates);
            return probability >= 0.5;
        }
    }
}
