using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBot.Core.Configuration;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Extractors;

namespace GameBot.Game.Tetris.Extraction.Extractors
{
    public class BlockBasedExtractor : IExtractor
    {
        private readonly TetrisExtractor _tetrisExtractor;

        public BlockBasedExtractor()
        {
            _tetrisExtractor = new TetrisExtractor(new AppSettingsConfig());
        }

        public bool ConfirmPiece(IScreenshot screenshot, Piece piece)
        {
            throw new NotImplementedException();
        }

        public Tetrimino? ExtractCurrentPiece(IScreenshot screenshot)
        {
            const int searchHeight = 15;

            return _tetrisExtractor.ExtractSpawnedPiece(screenshot, searchHeight)?.Tetrimino;
        }

        public Tetrimino? ExtractNextPiece(IScreenshot screenshot)
        {
            return _tetrisExtractor.ExtractNextPiece(screenshot);
        }
    }
}
