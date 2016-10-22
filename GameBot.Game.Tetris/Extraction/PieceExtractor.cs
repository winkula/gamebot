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

        /// <summary>
        /// Extracts the current piece from the board.
        /// </summary>
        /// <param name="screenshot">The screenshot to extract the piece from.</param>
        /// <param name="maxFallingDistance">Expected maximal falling distance.</param>
        /// <returns>The piece and it's probability.</returns>
        public ProbabilisticResult<Piece> ExtractPieceFuzzy(IScreenshot screenshot, int maxFallingDistance)
        {
            if (screenshot == null)
                throw new ArgumentNullException(nameof(screenshot));
            if (maxFallingDistance < 0)
                throw new ArgumentException("maxFallingDistance must be positive");

            double bestProbability = 0;
            Piece expectedPiece = null;

            for (int yDelta = 0; yDelta <= maxFallingDistance; yDelta++)
            {
                foreach (var tetromino in Tetriminos.All)
                {
                    foreach (var pose in tetromino.GetPoses())
                    {
                        var piece = pose.Fall(yDelta);
                        var probability = _pieceMatcher.GetProbability(screenshot, piece);
                        if (probability > bestProbability)
                        {
                            bestProbability = probability;
                            expectedPiece = piece;
                        }
                    }
                }
            }

            return new ProbabilisticResult<Piece>(expectedPiece, bestProbability);
        }

        /// <summary>
        /// Extracts the current piece from the board.
        /// The piece is only searched in the original orientation and x coordinate, must therefore be untouched.
        /// </summary>
        /// <param name="screenshot">The screenshot to extract the piece from.</param>
        /// <param name="maxFallingDistance">Expected maximal falling distance.</param>
        /// <returns>The piece and it's probability.</returns>
        public ProbabilisticResult<Piece> ExtractSpawnedPieceFuzzy(IScreenshot screenshot, int maxFallingDistance)
        {
            if (screenshot == null)
                throw new ArgumentNullException(nameof(screenshot));
            if (maxFallingDistance < 0)
                throw new ArgumentException("maxFallingDistance must be positive");

            double bestProbability = 0;
            Piece expectedPiece = null;

            for (int yDelta = 0; yDelta <= maxFallingDistance; yDelta++)
            {
                foreach (var tetromino in Tetriminos.All)
                {
                    var piece = new Piece(tetromino, 0, 0, -yDelta);
                    var probability = _pieceMatcher.GetProbability(screenshot, piece);
                    if (probability > bestProbability)
                    {
                        bestProbability = probability;
                        expectedPiece = piece;
                    }
                }
            }

            return new ProbabilisticResult<Piece>(expectedPiece, bestProbability);
        }

        /// <summary>
        /// Extracts the current piece from the board.
        /// </summary>
        /// <param name="screenshot">The screenshot to extract the piece from.</param>
        /// <param name="piece">The piece to search.</param>
        /// <param name="maxFallingDistance">Expected maximal falling distance.</param>
        /// <returns>The piece and it's probability.</returns>
        public ProbabilisticResult<Piece> ExtractKnownPieceFuzzy(IScreenshot screenshot, Piece piece, int maxFallingDistance)
        {
            if (screenshot == null)
                throw new ArgumentNullException(nameof(screenshot));
            if (maxFallingDistance < 0)
                throw new ArgumentException("maxFallingDistance must be positive");

            double bestProbability = 0;
            Piece expectedPiece = null;
            Piece testPiece = new Piece(piece);

            for (int yDelta = 0; yDelta <= maxFallingDistance; yDelta++)
            {
                var probability = _pieceMatcher.GetProbability(screenshot, testPiece);

                if (probability > bestProbability)
                {
                    bestProbability = probability;
                    expectedPiece = new Piece(testPiece);
                }

                testPiece.Fall();
            }

            return new ProbabilisticResult<Piece>(expectedPiece, bestProbability);
        }

        #endregion

        #region Next piece

        /// <summary>
        /// Extracts the next piece visible on the screenshot.
        /// Throws an exception, if the next piece is not visible on the screenshot.
        /// </summary>
        /// <param name="screenshot">The screenshot to extract the piece from.</param>
        /// <returns>The next Tetrimino.</returns>
        public Tetrimino ExtractNextPiece(IScreenshot screenshot)
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
            if (nextPiece?.Tetrimino == null)
                throw new ApplicationException("Next piece not visible on the screenshot");

            return nextPiece.Tetrimino;
        }

        /// <summary>
        /// Extracts the next piece visible on the screenshot.
        /// </summary>
        /// <param name="screenshot">The screenshot to extract the piece from.</param>
        /// <returns>The next Tetrimino.</returns>
        public ProbabilisticResult<Tetrimino> ExtractNextPieceFuzzy(IScreenshot screenshot)
        {
            // relevant tiles on the screen: x : 14 - 17, y : 13 - 16 

            double bestProbability = 0;
            Tetrimino? bestTetrimino = null;

            foreach (var tetrimino in Tetriminos.All)
            {
                //var piece = new Piece(tetromino, 0, TetrisConstants.NextPieceTemplateTileCoordinates.X, TetrisConstants.NextPieceTemplateTileCoordinates.Y);
                //var probability = _pieceMatcher.GetProbability(screenshot, piece);
                var probability = _pieceMatcher.GetProbabilityNextPiece(screenshot, tetrimino);
                if (probability > bestProbability)
                {
                    bestProbability = probability;
                    bestTetrimino = tetrimino;
                }
            }

            if (!bestTetrimino.HasValue) throw new Exception("No possible value for tetrimino found!");
            return new ProbabilisticResult<Tetrimino>(bestTetrimino.Value, bestProbability);
        }

        #endregion

        private bool IsBlock(byte mean)
        {
            // TODO: better threshold here?
            return mean < 255;
        }
    }
}
