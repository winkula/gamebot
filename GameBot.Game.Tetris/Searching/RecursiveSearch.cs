﻿using GameBot.Game.Tetris.Data;
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
            result.CurrentGameState = root?.GameState;
            if (goal != null)
            {
                result.GoalGameState = goal?.GameState;
                result.Way = goal?.Moves;
                result.Moves = GetMoves(result.Way);
            }

            return result;
        }

        protected Node SearchRecursive(Node parent, int depth)
        {
            if (depth >= this.depth)
            {
                // score

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

                double expected = double.NegativeInfinity;
                bestNode = parent;

                // test each tetromino with its chance to appear
                foreach (var tetromino in tetrominos)
                {
                    double chance = tetromino.GetChance();

                    //TetrisNode probabBestNode = null;
                    double probabBestScore = double.NegativeInfinity;

                    var newState = new GameState(parent.GameState, new Piece(tetromino));
                    var newNode = new Node(newState, parent);

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
