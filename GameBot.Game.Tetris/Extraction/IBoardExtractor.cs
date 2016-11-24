using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public interface IBoardExtractor
    {
        Board UpdateMultiplayer(IScreenshot screenshot, Board board);
    }
}
