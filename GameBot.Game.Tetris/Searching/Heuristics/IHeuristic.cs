namespace GameBot.Game.Tetris.Searching.Heuristics
{
    public interface IHeuristic
    {
        double Score(TetrisGameState gameState);
    }
}
