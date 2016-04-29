using GameBot.Core.Searching;
using GameBot.Game.Tetris.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                if (best?.Score > bestScore)
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
                //var score = ScoreProbabilisticExpected(successor);
                //var score = ScoreProbabilisticMinimum(successor);
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

        protected double ScoreProbabilisticExpected(TetrisNode parent)
        {
            var tetrominos = Enum.GetValues(typeof(Tetromino)).Cast<Tetromino>();
            double expectation = 0;
            
            foreach (var tetromino in tetrominos)
            {
                expectation += ScoreByChance(parent, tetromino);
            }

            return expectation;
        }

        protected double ScoreProbabilisticMinimum(TetrisNode parent)
        {
            var tetrominos = Enum.GetValues(typeof(Tetromino)).Cast<Tetromino>();
            double minimum = 0;

            foreach (var tetromino in tetrominos)
            {
                var newState = new TetrisGameState(parent.GameState, new Piece(tetromino));
                var child = new TetrisNode(newState);
                var successors = child.GetSuccessors();
                var score = GetBestScore(successors);

                minimum = Math.Min(minimum, score);
            }

            return minimum;
        }

        protected double ScoreByChance(TetrisNode parent, Tetromino tetromino)
        {
            double chance = tetromino.GetChance();

            var newState = new TetrisGameState(parent.GameState, new Piece(tetromino));
            var child = new TetrisNode(newState);
            var successors = child.GetSuccessors();
            var score = GetBestScore(successors);

            return chance * score;
        }

        protected double GetBestScore(IEnumerable<TetrisNode> successors)
        {
            var bestScore = double.NegativeInfinity;

            foreach (var successor in successors)
            {
                var score = heuristic.Score(successor.GameState);
                bestScore = Math.Max(bestScore, score);
            }

            return bestScore;
        }
    }
}
