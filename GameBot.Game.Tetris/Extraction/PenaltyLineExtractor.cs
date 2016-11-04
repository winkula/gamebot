using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction.Matchers;

namespace GameBot.Game.Tetris.Extraction
{
    public class PenaltyLineExtractor : IPenaltyLineExtractor
    {
        private readonly IMatcher _matcher = new MorphologyMatcher();

        public Board Update(IScreenshot screenshot, Board board)
        {
            var newBoard = new Board(board);
            double p = _matcher.GetProbability(screenshot, 5, 4);

            return board;
        }
    }
}
