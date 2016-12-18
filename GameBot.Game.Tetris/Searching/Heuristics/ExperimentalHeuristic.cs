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
            CalculateFast(board);

            /*
            var a = CalculatedAggregateHeight;
            var lines = gameState.Lines;
            var h = CalculatedHoles;
            var b = BumpinessWithoutLastColumn(board);
            */

            

            var score = 
                -0.510066 * CalculatedAggregateHeight
                + 0.760666 * gameState.Lines
                - 0.7 * CalculatedHoles
                - 0.184483 * BumpinessWithoutLastColumn(board);

            int tetrisBonus = 0;
            if (gameState.Lines == 3)
            {
                tetrisBonus = 100;
            }
            else if (gameState.Lines == 4)
            {
                tetrisBonus = 300;
            }
            var penalty = -80 * board.ColumnHeight(9);

            return score + penalty + tetrisBonus;
        }
    }
}
