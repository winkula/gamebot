using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction.Extractors
{
    public interface IExtractor
    {
        bool ConfirmPiece(IScreenshot screenshot, Piece piece);

        Tetrimino? ExtractCurrentPiece(IScreenshot screenshot);

        Tetrimino? ExtractNextPiece(IScreenshot screenshot);
    }
}
