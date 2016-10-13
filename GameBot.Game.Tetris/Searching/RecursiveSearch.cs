using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris.Searching
{
    public class RecursiveSearch : BaseSearch
    {
        private readonly int depth;
        private readonly ICollection<Tetromino> tetrominos;

        public RecursiveSearch(IHeuristic heuristic, int depth) : base(heuristic)
        {
            this.depth = depth;
            this.tetrominos = Enum.GetValues(typeof(Tetromino)).Cast<Tetromino>().ToList();
        }

        public override SearchResult Search(GameState gameState)
        {
            if (gameState == null)
                throw new ArgumentNullException(nameof(gameState));

            var root = new Node(gameState);
            var goal = SearchRecursive(root, 0);
            
            var result = new SearchResult();
            result.CurrentGameState = gameState;
            if (goal != null)
            {
                result.GoalGameState = goal?.GameState;
                result.Way = GetWayToNextSuccessor(goal);
                result.Moves = GetMoves(result.Way);
            }

            return result;
        }

        private Way GetWayToNextSuccessor(Node goal)
        {
            Way way = null;

            Node parent = goal;
            while (parent != null && parent.Way != null)
            {
                way = parent.Way;
                parent = parent.Parent;
            }

            return way;
        }

        protected Node SearchRecursive(Node parent, int depth)
        {
            if (depth >= this.depth)
            {
                // depth limit is reached -> score the game state
                parent.Score = heuristic.Score(parent.GameState);
                return parent;
            }

            Node bestNode = null;
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

                double expected = 0;
                bestNode = parent;

                // test each tetromino with its chance to appear
                foreach (var tetromino in tetrominos)
                {
                    double chance = tetromino.GetChance();

                    //Node bestNodeForThisTetromino = null;
                    double bestScoreForThisTetromino = double.NegativeInfinity;

                    var newState = new GameState(parent.GameState, new Piece(tetromino));
                    var newNode = new Node(newState, parent);

                    foreach (var successor in newNode.GetSuccessors())
                    {
                        var best = SearchRecursive(successor, depth + 1);
                        if (best?.Score > bestScoreForThisTetromino)
                        {
                            //bestNodeForThisTetromino = best;
                            bestScoreForThisTetromino = best.Score;
                        }
                    }

                    expected += chance * bestScoreForThisTetromino;
                }

                bestNode.Score = expected;
            }

            return bestNode;
        }
    }
}
