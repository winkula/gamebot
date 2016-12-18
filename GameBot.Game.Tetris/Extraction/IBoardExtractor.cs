using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public interface IBoardExtractor
    {
        bool IsHorizonBroken(IScreenshot screenshot, Board board);

        int MultiplayerRaisedLines(IScreenshot screenshot, Board board);

        ProbabilisticResult<int> MultiplayerHolePosition(IScreenshot screenshot, Board board);
        
        Board MultiplayerAddLines(Board board, int raisedLines, int holePosition);

        Board Update(IScreenshot screenshot, Board board, Piece piece);
    }
}
