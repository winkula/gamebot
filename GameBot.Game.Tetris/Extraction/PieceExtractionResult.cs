using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public class PieceExtractionResult
    {
        public Piece Piece { get; protected set; }
        public int Errors { get; protected set; }

        public PieceExtractionResult(Piece piece, int errors = 0)
        {
            Piece = piece;
            Errors = errors;
        }
    }
}
