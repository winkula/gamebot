using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;

namespace GameBot.Game.Tetris.Extraction.Extractors
{
    public abstract class BaseExtractor : IExtractor
    {
        protected readonly double ThresholdNextPiece;
        protected readonly double ThresholdCurrentPiece;
        protected readonly double ThresholdMovedPiece;

        protected readonly IMatcher Matcher;
        private readonly PieceExtractorBase _pieceExtractor;

        protected BaseExtractor(IConfig config, IMatcher matcher)
        {
            ThresholdNextPiece = config.Read<double>("Game.Tetris.Extractor.ThresholdNextPiece");
            ThresholdCurrentPiece = config.Read<double>("Game.Tetris.Extractor.ThresholdCurrentPiece");
            ThresholdMovedPiece = config.Read<double>("Game.Tetris.Extractor.ThresholdMovedPiece");

            Matcher = matcher;
            _pieceExtractor = new PieceExtractorBase(matcher);
        }
        
        public virtual Tetrimino? ExtractNextPiece(IScreenshot screenshot)
        {
            var result = _pieceExtractor.ExtractNextPieceFuzzy(screenshot);
            if (result.IsAccepted(ThresholdNextPiece))
            {
                return result.Result;
            }

            return null;
        }

        public virtual Piece ExtractCurrentPiece(IScreenshot screenshot, Tetrimino? tetrimino, int maxFallDistance)
        {
            if (tetrimino.HasValue)
            {
                var piece = new Piece(tetrimino.Value);
                var resultKnown = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, piece, maxFallDistance);
                if (resultKnown.IsAccepted(ThresholdCurrentPiece))
                {
                    return resultKnown.Result;
                }
            }
            
            var result = _pieceExtractor.ExtractSpawnedPieceFuzzy(screenshot, maxFallDistance);
            if (result.IsAccepted(ThresholdCurrentPiece))
            {
                return result.Result;
            }

            return null;
        }

        public virtual Piece ExtractMovedPiece(IScreenshot screenshot, Piece piece, Move move, int maxFallDistance, out bool moved)
        {
            var pieceMoved = new Piece(piece).Apply(move);

            var resultNotMoved = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, piece, maxFallDistance);
            var resultMoved = _pieceExtractor.ExtractKnownPieceFuzzy(screenshot, pieceMoved, maxFallDistance);

            if (resultMoved.IsAccepted(ThresholdMovedPiece) && resultMoved.Probability >= resultNotMoved.Probability)
            {
                moved = true;
                return resultMoved.Result;
            }
            if (resultNotMoved.IsAccepted(ThresholdMovedPiece) && resultNotMoved.Probability > resultMoved.Probability)
            {
                moved = false;
                return resultNotMoved.Result;
            }

            moved = false;
            return null;
        }
    }
}
