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

            var a = AggregateHeightLastMultiple(board, 99);
            var c = gameState.Lines;
            var h = Holes(board);
            var b = BumpinessWithoutLastColumn(board);
            var m = board.MaximumHeight;

            return -0.510066 * a
                /*+ 0.760666 * c*/
                - 0.35663 * h
                - 0.184483 * b;
        }
    }
}
