using System;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Searching.Heuristics
{
    public class ExperimentalHeuristic : BasicTetrisHeuristic
    {
        // Heuristic from here: http://www.diva-portal.se/smash/get/diva2:815662/FULLTEXT01.pdf
        // Modified to get less holes
        public override double Score(GameState gameState)
        {
            var board = gameState.Board;

            //var a = AggregateHeightLastMultiple(board, 99);
            //var c = gameState.Lines;
            var h = Holes(board);
            var b = Bumpiness(board);
            var m = board.MaximumHeight;

            return -5 * h - b - 10 * Math.Pow(m, 2);
        }
    }
}
