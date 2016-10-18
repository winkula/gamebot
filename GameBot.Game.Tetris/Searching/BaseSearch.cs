﻿using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris.Searching
{
    // Tetris breadth first search
    public abstract class BaseSearch : ISearch
    {
        protected readonly IHeuristic Heuristic;

        protected BaseSearch(IHeuristic heuristic)
        {
            Heuristic = heuristic;
        }

        public virtual SearchResult Search(GameState gameState)
        {
            if (gameState == null)
                throw new ArgumentNullException(nameof(gameState));

            var gameStateBegin = new GameState(gameState).ResetLinesAndScore();

            Node root = new Node(gameStateBegin);            
            Node goal = null;
            var bestScore = double.NegativeInfinity;

            foreach (var successor1 in root.GetSuccessors())
            {
                Node best = null;
                var bestScore2 = double.NegativeInfinity;

                foreach (var successor2 in successor1.GetSuccessors())
                {
                    var score = Score(successor2);
                    if (score > bestScore2)
                    {
                        best = successor2;
                        best.Score = score;
                        bestScore2 = score;
                    }
                }
                
                if (best?.Score > bestScore)
                {
                    goal = best;
                    bestScore = best.Score;
                }
            }
            
            if (goal?.Parent != null)
            {
                var result = new SearchResult
                {
                    CurrentGameState = root.GameState,
                    GoalGameState = goal.Parent.GameState,
                    Way = goal.Parent.Way
                };
                result.Moves = GetMoves(result.Way);
                return result;
            }

            // game is in the wrong state
            // no search is possible here
            return null;
        }

        protected IEnumerable<Move> GetMoves(Way way)
        {
            if (way != null)
            {
                // rotation
                if (way.Rotation % 4 == 3)
                {
                    // counterclockwise rotation
                    yield return Move.RotateCounterclockwise;
                }
                else
                {
                    // clockwise rotation
                    for (int i = 0; i < way.Rotation % 4; i++)
                    {
                        yield return Move.Rotate;
                    }
                }

                // translation
                if (way.Translation < 0)
                {
                    // move left
                    for (int i = 0; i < -way.Translation; i++)
                    {
                        yield return Move.Left;
                    }
                }
                else if (way.Translation > 0)
                {
                    // move right
                    for (int i = 0; i < way.Translation; i++)
                    {
                        yield return Move.Right;
                    }
                }

                // drop
                yield return Move.Drop;
            }
        }

        protected virtual double Score(Node node)
        {
            return ScoreSimple(node);
        }

        private double ScoreSimple(Node parent)
        {
            return Heuristic.Score(parent.GameState);
        }

        protected double ScoreProbabilisticExpected(Node parent)
        {
            return Enum.GetValues(typeof(Tetromino))
                .Cast<Tetromino>()
                .AsParallel()
                .Sum(x => ScoreByChance(parent, x));
        }

        protected double ScoreProbabilisticMinimum(Node parent)
        {
            var tetrominos = Enum.GetValues(typeof(Tetromino)).Cast<Tetromino>();
            double minimum = 0;

            foreach (var tetromino in tetrominos)
            {
                var newState = new GameState(parent.GameState, new Piece(tetromino));
                var child = new Node(newState);
                var successors = child.GetSuccessors();
                var score = GetBestScore(successors);

                minimum = Math.Min(minimum, score);
            }

            return minimum;
        }

        protected double ScoreByChance(Node parent, Tetromino tetromino)
        {
            double chance = tetromino.GetChance();

            var newState = new GameState(parent.GameState, new Piece(tetromino));
            var child = new Node(newState);
            var successors = child.GetSuccessors();
            var score = GetBestScore(successors);

            return chance * score;
        }

        protected double GetBestScore(IEnumerable<Node> successors)
        {
            var bestScore = double.NegativeInfinity;

            foreach (var successor in successors)
            {
                var score = Heuristic.Score(successor.GameState);
                bestScore = Math.Max(bestScore, score);
            }

            return bestScore;
        }
    }
}
