using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using System;

namespace GameBot.Game.Tetris.Extraction
{
    public class PieceMatcher
    {
        // singleton (database)
        private static PieceMatcher _instance;
        public static PieceMatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PieceMatcher();
                }
                return _instance;
            }
        }

        private PieceMatcher()
        {
        }

        public int GetErrors(IScreenshot screenshot, Piece expected)
        {
            int errors = 0;
            foreach (var block in expected.Shape.Body)
            {
                var coordinates = Coordinates.PieceToTile(expected.X + block.X, expected.Y + block.Y);
                var mean = screenshot.GetTileMean(coordinates.X, coordinates.Y);
                if (!TetrisExtractor.IsBlock(mean))
                {
                    errors++;
                }
            }
            return errors;
        }

        public double GetProbability(ushort mask, Piece expected)
        {
            throw new NotImplementedException();
        }
    }
}
