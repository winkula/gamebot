using System;
using System.Diagnostics;
using System.Linq;

namespace GameBot.Core.Searching
{
    public class DepthFirstSearch
    {
        public int Bound { get; private set; }

        public DepthFirstSearch(int bound)
        {
            Bound = bound;
        }

        public DepthFirstSearch() : this(int.MaxValue)
        {
        }

        public INode Search(INode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            return Search(node, 0);
        }

        protected INode Search(INode parent, int level)
        {
            if (level > Bound)
            {
                return parent;
            }

            var successors = parent.GetSuccessors();
            if (successors.Any())
            {
                INode winner = null;
                var bestScore = double.NegativeInfinity;
                foreach (var successor in successors)
                {
                    var child = Search(successor, level + 1);
                    var score = child.GetScore();
                    if (winner == null || score > bestScore)
                    {
                        winner = child;
                        bestScore = score;
                    }
                }

                return winner;
            }

            return parent;
        }
    }
}
