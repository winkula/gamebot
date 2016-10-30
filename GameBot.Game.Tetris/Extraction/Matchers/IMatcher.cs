using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction.Matchers
{
    public interface IMatcher
    {
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
