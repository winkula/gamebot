using System;
using System.Drawing;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public class PieceExtractor
    {
        // this coordinates are in the coordinate system of the tile system of the game boy screen (origin is top left)
        private static Point _currentTileOrigin = new Point(5, 0);
        private static Point _previewTileOrigin = new Point(15, 13);

        #region Current piece
        


        #endregion
        
        #region Next piece

        /// <summary>
        /// Extracts the next piece visible on the screenshot.
        /// Throws an exception, if the next piece is not visible on the screenshot.
        /// </summary>
        /// <param name="screenshot">The screenshot to extract the piece from.</param>
        /// <returns>The next Tetromino.</returns>
        public Tetromino ExtractNextPiece(IScreenshot screenshot)
        {
            // relevant tiles on the screen: x : 14 - 17, y : 13 - 16 

            ushort mask = 0;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    byte mean = screenshot.GetTileMean(_previewTileOrigin.X + x, _previewTileOrigin.Y + y);
                    if (IsBlock(mean))
                    {
                        int index = 4 * (2 - y) + (x);
                        mask |= (ushort)(1 << index);
                    }
                }
            }

            var nextPiece = Piece.FromMask(mask);
            if (nextPiece?.Tetromino == null)
                throw new ApplicationException("Next piece not visible on the screenshot");

            return nextPiece.Tetromino;
        }

        /// <summary>
        /// Extracts the next piece visible on the screenshot.
        /// </summary>
        /// <param name="screenshot"></param>
        /// <param name="probabilityThreshold"></param>
        /// <returns></returns>
        public Tetromino? ExtractNextPieceFuzzy(IScreenshot screenshot, double probabilityThreshold)
        {
            throw new NotImplementedException();
        }

        #endregion

        private bool IsBlock(byte mean)
        {
            return mean < 255;
        }
    }
}
