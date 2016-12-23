using GameBot.Core.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public interface IScreenExtractor
    {
        /// <summary>
        /// Is the start screeen visible?
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <returns><code>true</code>, wehen the start screen is visible.</returns>
        bool IsStart(IScreenshot screenshot);

        /// <summary>
        /// Is the game over screen in single player mode visible?
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <returns><code>true</code>, wehen the game over screen is visible.</returns>
        bool IsGameOverSingleplayer(IScreenshot screenshot);

        /// <summary>
        /// Is the  game over screeen in multiplayer mode visible?
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <returns><code>true</code>, wehen the game over screen is visible.</returns>
        bool IsGameOverMultiplayer(IScreenshot screenshot);
    }
}
