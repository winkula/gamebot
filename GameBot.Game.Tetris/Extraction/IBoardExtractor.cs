using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public interface IBoardExtractor
    {
        /// <summary>
        /// Checks whether the horizon of the internal game state is broken. If yes, we have to update the internal board state.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="board">The internal board state.</param>
        /// <returns><code>true</code>, if the horiozon is broken, i.e. there was an error in execution an action. <code>false</code> otherwise.</returns>
        bool IsHorizonBroken(IScreenshot screenshot, Board board);

        /// <summary>
        /// Extracts the number of raised lines in multiplayer mode.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="board">The internal board state.</param>
        /// <returns>The number of raised lines.</returns>
        int MultiplayerRaisedLines(IScreenshot screenshot, Board board);

        /// <summary>
        /// Extracts the position of the reised lines.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="board">The internal board state.</param>
        /// <returns>Probabilistic result of the hole position (for sampling).</returns>
        ProbabilisticResult<int> MultiplayerHolePosition(IScreenshot screenshot, Board board);

        /// <summary>
        /// Updates the internal board state according to the screenshot.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="board">The internal board state.</param>
        /// <param name="piece">The current piece.</param>
        /// <returns>The new internal board state.</returns>
        Board Update(IScreenshot screenshot, Board board, Piece piece);
    }
}
