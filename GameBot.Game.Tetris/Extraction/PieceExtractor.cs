using System;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public class PieceExtractor
    {
        private readonly PieceMatcher _pieceMatcher;

        public PieceExtractor(PieceMatcher pieceMatcher)
        {
            _pieceMatcher = pieceMatcher;
        }

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
                    byte mean = screenshot.GetTileMean(TetrisConstants.NextPieceTileOrigin.X + x, TetrisConstants.NextPieceTileOrigin.Y + y);
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
        /// <param name="screenshot">The screenshot to extract the piece from.</param>
        /// <param name="probabilityThreshold">Probability that must be reached.</param>
        /// <returns>The next Tetromino, or null if no matching piece was found.</returns>
        public Tetromino? ExtractNextPieceFuzzy(IScreenshot screenshot, double probabilityThreshold = 0.0)
        {
            // relevant tiles on the screen: x : 14 - 17, y : 13 - 16 

            if (probabilityThreshold < 0.0 || probabilityThreshold > 1.0)
                throw new ArgumentException("probabilityThreshold must be between 0.0 and 1.0");

            double bestProbability = 0;
            Tetromino? bestTetromino = null;
            
            foreach (var tetromino in Tetrominos.All)
            {
                var piece = new Piece(tetromino, 0, TetrisConstants.NextPieceTemplateTileCoordinates.X, TetrisConstants.NextPieceTemplateTileCoordinates.Y);
                var probability = _pieceMatcher.GetProbability(screenshot, piece);
                if (probability > bestProbability && probability >= probabilityThreshold)
                {
                    bestProbability = probability;
                    bestTetromino = tetromino;
                }
            }

            return bestTetromino;
        }

        #endregion

        private bool IsBlock(byte mean)
        {
            // TODO: better threshold here?
            return mean < 255;
        }
    }
}
