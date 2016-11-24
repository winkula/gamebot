using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public interface IBoardExtractor
    {
        bool IsHorizonBroken(IScreenshot screenshot, Board board);

        Board UpdateMultiplayer(IScreenshot screenshot, Board board);

        Board Update(IScreenshot screenshot, Board board, Piece piece);
    }
}
