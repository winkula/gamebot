using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public interface IPenaltyLineExtractor
    {
        Board Update(IScreenshot screenshot, Board board);
    }
}
