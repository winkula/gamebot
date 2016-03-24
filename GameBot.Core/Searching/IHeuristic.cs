namespace GameBot.Core.Searching
{
    public interface IHeuristic
    {
    }

    public interface IHeuristic<T> : IHeuristic
    {
        double Score(T gameState);
    }
}
