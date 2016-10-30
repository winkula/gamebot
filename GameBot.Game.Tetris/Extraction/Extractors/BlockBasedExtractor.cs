using GameBot.Core.Configuration;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction.Extractors
{
    public class BlockBasedExtractor : IExtractor
    {
        private readonly TetrisExtractor _tetrisExtractor;

        public BlockBasedExtractor()
        {
            _tetrisExtractor = new TetrisExtractor(new AppSettingsConfig());
        }
        
        public Tetrimino? ExtractNextPiece(IScreenshot screenshot)
        {
            return _tetrisExtractor.ExtractNextPiece(screenshot);
        }

        public Piece ExtractCurrentPiece(IScreenshot screenshot, Tetrimino? tetrimino, int maxFallDistance)
        {
            var foundPiece = _tetrisExtractor.ExtractSpawnedPiece(screenshot, maxFallDistance);
            if (foundPiece != null && tetrimino.HasValue && foundPiece.Tetrimino == tetrimino.Value)
            {
                return foundPiece;
            }
            return null;
        }

        public Piece ExtractMovedPiece(IScreenshot screenshot, Piece piece, Move move, int maxFallDistance, out bool moved)
        {
            return _tetrisExtractor.ExtractMovedPieceWithErrorTolerance(screenshot, piece, move, maxFallDistance, out moved);
        }
    }
}
