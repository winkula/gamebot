using System;
using System.Collections.Generic;
using GameBot.Game.Tetris.Searching.Heuristics;

namespace GameBot.Game.Tetris.Searching
{
    public class PredictiveSearch : BaseSearch
    {
        public bool Cache { get; set; }
        public int ScoreCalculated { get; private set; }
        public int ScoreLookedUp { get; private set; }

        private readonly Dictionary<Node, double> _dictionary;

        public PredictiveSearch(IHeuristic heuristic) : base(heuristic)
        {
            _dictionary = new Dictionary<Node, double>(4194304);
        }

        protected override double Score(Node node)
        {
            if (Cache)
            {
                double score;
                if (_dictionary.TryGetValue(node, out score))
                {
                    // found
                    ScoreLookedUp++;
                    return score;
                }

                // not found
                ScoreCalculated++;
                score = ScoreProbabilisticMinimum(node);
                _dictionary.Add(node, score);
                return score;
            }

            ScoreCalculated++;
            return ScoreProbabilisticMinimum(node);
        }
    }
}
