using GameBot.Game.Tetris.Searching.Heuristics;

namespace GameBot.Game.Tetris.Searching
{
    public class ProbabilisticSearch : BaseSearch
    {
        public ProbabilisticSearch(IHeuristic heuristic) : base(heuristic)
        {
        }

        protected override double Score(Node node)
        {
            return ScoreProbabilisticExpected(node);
        }
    }
}
