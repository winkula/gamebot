using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction.Extractors
{
    public class BlockBasedExtractor : IExtractor
    {
        private readonly TetrisExtractor _tetrisExtractor;

        public BlockBasedExtractor(IConfig config)
        {
            _tetrisExtractor = new TetrisExtractor(config);
        }
        
        public Tetrimino? ExtractNextPiece(IScreenshot screenshot)
        {
            return _tetrisExtractor.ExtractNextPiece(screenshot);
        }

        public Piece ExtractCurrentPiece(IScreenshot screenshot, Tetrimino? tetrimino, int maxFallDistance)
        {
            if (tetrimino.HasValue)
            {
                return ExtractCurrentPieceKnown(screenshot, tetrimino.Value, maxFallDistance);
            }
            return ExtractCurrentPieceUnknown(screenshot, maxFallDistance);
        }

        private Piece ExtractCurrentPieceKnown(IScreenshot screenshot, Tetrimino tetrimino, int maxFallDistance)
        {
            var foundPiece = _tetrisExtractor.ExtractSpawnedPiece(screenshot, maxFallDistance);
            if (foundPiece != null && foundPiece.Tetrimino == tetrimino && foundPiece.IsUntouched)
            {
                return foundPiece;
            }
            return null;
        }

        private Piece ExtractCurrentPieceUnknown(IScreenshot screenshot, int maxFallDistance)
        {
            return _tetrisExtractor.ExtractSpawnedPiece(screenshot, maxFallDistance);
        }

        public Piece ExtractMovedPiece(IScreenshot screenshot, Piece piece, Move move, int maxFallDistance, out bool moved)
        {
            return _tetrisExtractor.ExtractMovedPieceWithErrorTolerance(screenshot, piece, move, maxFallDistance, out moved);
        }
    }
}
