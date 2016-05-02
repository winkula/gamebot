namespace GameBot.Game.Tetris.Heuristics
{
    public class MaxBergmarkHeuristic : BasicTetrisHeuristic
    {
        // Heuristic from here: http://www.diva-portal.se/smash/get/diva2:815662/FULLTEXT01.pdf
        public override double Score(TetrisGameState gameState)
        {
            return -HolesValue(gameState.Board, (h => h*h), (h => h));
        }
    }
}
