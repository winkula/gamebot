using GameBot.Game.Tetris.Searching.Heuristics;

namespace GameBot.Game.Tetris.Searching
{
    public class SimpleSearch : BaseSearch
    {
        public SimpleSearch(IHeuristic heuristic) : base(heuristic)
        {
        }
    }
}
