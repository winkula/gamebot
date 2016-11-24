using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public interface IBoardExtractor
    {
        Board MultiplayerUpdate(IScreenshot screenshot, Board board);
    }
}
