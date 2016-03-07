using GameBot.Core.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Game.Tetris
{
    public class TetrisHeuristic : IHeuristic<TetrisGameState>
    {
        public double Score(TetrisGameState gameState)
        {
            return 1.0 / gameState.Board.AggregateHeight;
        }
    }
}
