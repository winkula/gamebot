namespace GameBot.Game.Tetris.Searching.Heuristics
{
    public class YiyuanLeeHeuristic : BasicTetrisHeuristic
    {
        // Heuristic from here: https://codemyroad.wordpress.com/2013/04/14/tetris-ai-the-near-perfect-player/
        public override double Score(TetrisGameState gameState)
        {
            var board = gameState.Board;

            var a = AggregateHeight(board);
            var c = gameState.Lines;
            var h = Holes(board);
            var b = Bumpiness(board);

            return -0.510066 * a + 0.760666 * c - 100 * h - 0.184483 * b;
            //return -0.510066 * a + 0.760666 * c - 0.35663 * h - 0.184483 * b;
        }
    }
}
