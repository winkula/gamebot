using GameBot.Core.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Game.Tetris
{
    // TODO: implement
    public class TetrisSearch : ISearch<TetrisNode>
    {
        private readonly IHeuristic heuristic;

        public TetrisSearch(IHeuristic heuristic)
        {
            this.heuristic = heuristic;
        }

        public TetrisNode Search(TetrisNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            return SearchCurrentPiece(node);
        }
        
        protected TetrisNode SearchCurrentPiece(TetrisNode parent)
        {
            throw new NotImplementedException();
            /*
            var successors = parent.GetSuccessors();
            if (successors.Any())
            {
                TetrisNode winner = null;
                var bestScore = double.NegativeInfinity;
                foreach (var successor in successors)
                {
                    var child = Search((TetrisNode)successor);
                    var score = child.GetScore();
                    if (winner == null || score > bestScore)
                    {
                        winner = child;
                        bestScore = score;
                    }
                }

                return winner;
            }

            return parent;*/
        }

        protected TetrisNode SearchNextPiece(TetrisNode parent)
        {
            throw new NotImplementedException();
        }

        protected TetrisNode SearchGuessedPiece(TetrisNode parent)
        {
            throw new NotImplementedException();
        }

        protected double RateBoard(TetrisNode parent)
        {
            throw new NotImplementedException();
        }
    }
}
