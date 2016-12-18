using GameBot.Core.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public interface IScreenExtractor
    {
        bool IsGameOverSingleplayer(IScreenshot screenshot);

        bool IsGameOverMultiplayer(IScreenshot screenshot);

        bool IsStart(IScreenshot screenshot);
    }
}
