namespace GameBot.Game.Tetris.Searching.Heuristics
{
    public class ExperimentalHeuristic : BasicTetrisHeuristic
    {
        // Heuristic from here: http://www.diva-portal.se/smash/get/diva2:815662/FULLTEXT01.pdf
        // Slightly modified
        public override double Score(TetrisGameState gameState)
        {
            var board = gameState.Board;

            var valueHoles = -1 * HolesValueStacking(board, (h => 10 + h), (h => 1));
            var valueLines = 100 * Threshold(gameState.Lines, 3);
            var valueMaxHeight = -100 * Threshold(MaximumHeight(board), 6);

            return valueHoles + valueLines + valueMaxHeight;
        }
    }
}
