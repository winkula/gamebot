using GameBot.Core.Searching;
using GameBot.Game.Tetris.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris
{
    public class TetrisProbabilisticSearch : ISearch<TetrisNode>
    {
        private readonly IHeuristic<TetrisGameState> heuristic;
        private readonly int depth;
        private readonly ICollection<Tetromino> tetrominos;

        public TetrisProbabilisticSearch(IHeuristic<TetrisGameState> heuristic, int depth)
        {
            this.heuristic = heuristic;
            this.depth = depth;
            this.tetrominos = Enum.GetValues(typeof(Tetromino)).Cast<Tetromino>().ToList();
        }

        public TetrisNode Search(TetrisNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            return SearchRecursive(node, 0);
        }

        protected TetrisNode SearchRecursive(TetrisNode parent, int depth)
        {
            if (depth >= this.depth)
            {
                // score

                parent.Score = heuristic.Score(parent.GameState);
                return parent;
            }

            TetrisNode bestNode = null;
            double bestScore = double.NegativeInfinity;

            if (depth < 2)
            {
                // search deterministic

                foreach (var successor in parent.GetSuccessors())
                {
                    var best = SearchRecursive(successor, depth + 1);
                    if (best?.Score > bestScore)
                    {
                        bestNode = best;
                        bestScore = best.Score;
                    }
                }
                bestNode = bestNode ?? parent;
            }
            else
            {
                // search probabilistic

                double expected = double.NegativeInfinity;
                bestNode = parent;

                // test each tetromino with its chance to appear
                foreach (var tetromino in tetrominos)
                {
                    double chance = tetromino.GetChance();

                    //TetrisNode probabBestNode = null;
                    double probabBestScore = double.NegativeInfinity;

                    var newState = new TetrisGameState(parent.GameState, new Piece(tetromino));
                    var newNode = new TetrisNode(newState, parent);
                    
                    foreach (var successor in newNode.GetSuccessors())
                    {
                        var best = SearchRecursive(successor, depth + 1);
                        if (best?.Score > probabBestScore)
                        {
                            //probabBestNode = best;
                            probabBestScore = best.Score;
                        }
                    }

                    expected += chance * probabBestScore;
                }

                bestNode.Score = expected;
            }

            return bestNode;
        }  
    }
}
