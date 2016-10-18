using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Searching.Heuristics
{
    public class ExperimentalHeuristic : BasicTetrisHeuristic
    {
        // Heuristic from here: http://www.diva-portal.se/smash/get/diva2:815662/FULLTEXT01.pdf
        // Slightly modified
        public override double Score(GameState gameState)
        {
            var board = gameState.Board;

            var a = AggregateHeight(board);
            var c = gameState.Lines;
            var h = Holes(board);
            var b = Bumpiness(board);

            return -0.5 * a + 0.9 * c - 0.6 * h - 0.1 * b;
        }
    }
}
