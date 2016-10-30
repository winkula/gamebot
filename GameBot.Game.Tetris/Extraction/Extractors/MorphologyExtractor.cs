using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;

namespace GameBot.Game.Tetris.Extraction.Extractors
{
    public class MorphologyExtractor : IExtractor
    {
        private const double _thresholdNextPiece = 0.2;
        private const double _thresholdCurrentPiece = 0.5;
        private const double _thresholdMovedPiece = 0.5;

        private readonly PieceExtractorBase _pieceExtractor;

        public MorphologyExtractor()
        {
            var matcher = new MorphologyMatcher();
            _pieceExtractor = new PieceExtractorBase(matcher);
        }
        
        public Tetrimino? ExtractNextPiece(IScreenshot screenshot)
        {
            var result = _pieceExtractor.ExtractNextPieceFuzzy(screenshot);
            if (result.IsAccepted(_thresholdNextPiece))
            {
                return result.Result;
            }

            return null;
        }

        public Piece ExtractCurrentPiece(IScreenshot screenshot, Tetrimino? tetrimino, int maxFallDistance)
        {
            if (tetrimino.HasValue)
            {
                var piece = new Piece(tetrimino.Value);
                var resultKnown = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, piece, maxFallDistance);
                if (resultKnown.IsAccepted(_thresholdCurrentPiece))
                {
                    return resultKnown.Result;
                }
            }
            
            var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, maxFallDistance);
            if (result.IsAccepted(_thresholdCurrentPiece))
            {
                return result.Result;
            }

            return null;
        }

        public Piece ExtractMovedPiece(IScreenshot screenshot, Piece piece, Move move, int maxFallDistance, out bool moved)
        {
            var pieceMoved = new Piece(piece).Apply(move);

            var resultNotMoved = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, piece, maxFallDistance);
            var resultMoved = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, pieceMoved, maxFallDistance);

            if (resultMoved.IsAccepted(_thresholdMovedPiece) && resultMoved.Probability >= resultNotMoved.Probability)
            {
                moved = true;
                return resultMoved.Result;
            }
            if (resultNotMoved.IsAccepted(_thresholdMovedPiece) && resultNotMoved.Probability > resultMoved.Probability)
            {
                moved = false;
                return resultNotMoved.Result;
            }

            moved = false;
            return null;
        }
    }
}
