using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public interface IBoardExtractor
    {
        // TODO: move to own class? nothing to do with board
        bool IsGameOver(IScreenshot screenshot);

        bool IsHorizonBroken(IScreenshot screenshot, Board board);

        Board UpdateMultiplayer(IScreenshot screenshot, Board board);

        Board Update(IScreenshot screenshot, Board board, Piece piece);
    }
}
