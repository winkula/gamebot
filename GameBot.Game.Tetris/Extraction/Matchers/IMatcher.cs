using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction.Matchers
{
    public interface IMatcher
    {
        /// <summary>
        /// Gets the probability that a specific block is occupied.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="x">The x coordinate of the block.</param>
        /// <param name="y">The y coordinate of the block.</param>
        /// <returns>The probability.</returns>
        double GetProbability(IScreenshot screenshot, int x, int y);

        /// <summary>
        /// Gets the probability that a specific piece is visible on the board.     
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="piece">The piece to match.</param>
        /// <returns>The probability.</returns>
        double GetProbability(IScreenshot screenshot, Piece piece);

        /// <summary>
        /// Gets the probability that a specific piece is visible in the preview window.   
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="tetrimino">The tetrimino to match.</param>
        /// <returns>The probability.</returns>
        double GetProbabilityNextPiece(IScreenshot screenshot, Tetrimino tetrimino);
    }
}
