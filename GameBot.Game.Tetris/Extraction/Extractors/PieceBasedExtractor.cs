using System;
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
        
        public Tetrimino? ExtractNextPiece(IScreenshot screenshot)
        {
            const double threshold = 0.2;

            var result = _pieceExtractor.ExtractNextPieceFuzzy(screenshot);
            if (result.IsAccepted(threshold))
            {
                return result.Result;
            }

            return null;
        }

        public Piece ExtractCurrentPiece(IScreenshot screenshot, Tetrimino? tetrimino, int maxFallDistance)
        {
            const double threshold = 0.5;

            if (tetrimino.HasValue)
            {
                var piece = new Piece(tetrimino.Value);
                var resultKnown = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, piece, maxFallDistance);
                if (resultKnown.IsAccepted(threshold))
                {
                    return resultKnown.Result;
                }
            }
            
            var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, maxFallDistance);
            if (result.IsAccepted(threshold))
            {
                return result.Result;
            }

            return null;
        }

        public Piece ExtractMovedPiece(IScreenshot screenshot, Piece piece, Move move, int maxFallDistance, out bool moved)
        {
            throw new NotImplementedException();
        }
    }
}
