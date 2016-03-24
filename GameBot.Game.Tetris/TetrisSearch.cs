using GameBot.Core.Searching;
using GameBot.Game.Tetris.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris
{
    public class TetrisSearch : ISearch<TetrisNode>
    {
        private readonly IHeuristic<TetrisGameState> heuristic;

        public TetrisSearch(IHeuristic<TetrisGameState> heuristic)
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
            TetrisNode goal = null;
            var bestScore = double.NegativeInfinity;

            foreach (var successor in parent.GetSuccessors())
            {
                var best = SearchNextPiece(successor);
                if (best.Score > bestScore)
                {
                    goal = best;
                    bestScore = best.Score;
                }
            }

            return goal;
        }

        protected TetrisNode SearchNextPiece(TetrisNode parent)
        {
            TetrisNode goal = null;
            var bestScore = double.NegativeInfinity;

            foreach (var successor in parent.GetSuccessors())
            {
                //var score = ScoreProbabilistic(successor);
                var score = ScoreSimple(successor);
                if (score > bestScore)
                {
                    goal = successor;
                    goal.Score = score;
                    bestScore = score;
                }
            }

            return goal;
        }

        protected double ScoreSimple(TetrisNode parent)
        {
            return heuristic.Score(parent.GameState);
        }

        protected double ScoreProbabilistic(TetrisNode parent)
        {
            var tetrominos = Enum.GetValues(typeof(Tetromino)).Cast<Tetromino>();
            double expectation = 0;

            foreach (var tetromino in tetrominos)
            {
                double chance = tetromino.GetChance();

                var child = new TetrisGameState(parent.GameState.Board, new Piece(tetromino), null);
                var successors = child.GetSuccessors();
                var score = GetBestScore(successors);

                expectation += chance * score;
            }

            return expectation;
        }
                
        protected double GetBestScore(IEnumerable<TetrisGameState> successors)
        {
            var bestScore = double.NegativeInfinity;

            foreach (var successor in successors)
            {
                var score = heuristic.Score(successor);
                bestScore = Math.Max(bestScore, score);
            }

            return bestScore;
        }
    }
}
