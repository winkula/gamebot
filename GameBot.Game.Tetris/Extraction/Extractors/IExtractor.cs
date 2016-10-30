using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction.Extractors
{
    /// <summary>
    /// Represents an extractor for Tetris game state informations.
    /// </summary>
    public interface IExtractor
    {
        /// <summary>
        /// Extracts the next piece from the preview window.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <returns>The next piece or null, if not found.</returns>
        Tetrimino? ExtractNextPiece(IScreenshot screenshot);

        /// <summary>
        /// Extracts the current piece from the board. The tetrimino can be provided, if known.
        /// Only untouched (origin orientation an x coordinate) pieces are accepted.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="tetrimino">The known tetrimino.</param>
        /// <param name="maxFallDistance">The maximal fall distance.</param>
        /// <returns>The current piece or null, if not found.</returns>
        Piece ExtractCurrentPiece(IScreenshot screenshot, Tetrimino? tetrimino, int maxFallDistance);

        /// <summary>
        /// Extracts a piece and states, if it has been moved or not according to the specified move.
        /// This method is needed to track the movements of the current piece.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="piece">The current piece.</param>
        /// <param name="move">The move to confirm.</param>
        /// <param name="maxFallDistance">The maximal fall distance.</param>
        /// <param name="moved"><code>true</code>, when the piece was moved.</param>
        /// <returns>The new or old position of the piece or null, if not found.</returns>
        Piece ExtractMovedPiece(IScreenshot screenshot, Piece piece, Move move, int maxFallDistance, out bool moved);
    }
}
