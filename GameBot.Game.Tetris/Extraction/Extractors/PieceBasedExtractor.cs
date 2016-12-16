using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;

namespace GameBot.Game.Tetris.Extraction.Extractors
{
    public class PieceBasedExtractor : IExtractor
    {
        private readonly double _thresholdNextPiece;
        private readonly double _thresholdCurrentPiece;
        private readonly double _thresholdMovedPiece;

        private readonly PieceExtractor _pieceExtractor;

        public PieceBasedExtractor(IConfig config)
        {
            _thresholdNextPiece = config.Read("Game.Tetris.Extractor.ThresholdNextPiece", 0.2);
            _thresholdCurrentPiece = config.Read("Game.Tetris.Extractor.ThresholdCurrentPiece", 0.5);
            _thresholdMovedPiece = config.Read("Game.Tetris.Extractor.ThresholdMovedPiece", 0.5);

            var matcher = new TemplateMatcher();
            _pieceExtractor = new PieceExtractor(matcher);
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

        public bool DetectPiece(IScreenshot screenshot, int maxFallDistance)
        {
            throw new System.NotImplementedException();
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
