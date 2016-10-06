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
        
        public double GetProbability(ushort mask, Piece expected)
        {
            throw new NotImplementedException();
        }
    }
}
