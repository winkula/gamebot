using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction.Extractors
{
    public class PieceBasedExtractor : IExtractor
    {
        private readonly PieceMatcher _pieceMatcher;
        private readonly PieceExtractor _pieceExtractor;

        public PieceBasedExtractor()
        {
            _pieceMatcher = new PieceMatcher();
            _pieceExtractor = new PieceExtractor(_pieceMatcher);
        }

        public bool ConfirmPiece(IScreenshot screenshot, Piece piece)
        {
            const double threshold = 0.5;

            var probability = _pieceMatcher.GetProbability(screenshot, piece);
            return probability >= threshold;
        }

        public Tetrimino? ExtractCurrentPiece(IScreenshot screenshot)
        {
            const double threshold = 0.5;
            const int searchHeight = 15;

            var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, searchHeight);
            if (result.IsAccepted(threshold))
            {
                return result.Result?.Tetrimino;
            }
            return null;
        }

        public Tetrimino? ExtractNextPiece(IScreenshot screenshot)
        {
            const double threshold = 0.5;

            var result = _pieceExtractor.ExtractNextPieceFuzzy(screenshot);
            if (result.IsAccepted(threshold))
            {
                return result.Result;
            }
            return null;
        }
    }
}
