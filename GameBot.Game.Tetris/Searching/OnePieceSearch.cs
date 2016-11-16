using System;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching.Heuristics;

namespace GameBot.Game.Tetris.Searching
{
    public class OnePieceSearch : BaseSearch
    {
        public OnePieceSearch(IHeuristic heuristic) : base(heuristic)
        {
        }

        public override SearchResult Search(GameState gameState)
        {
            if (gameState == null)
                throw new ArgumentNullException(nameof(gameState));

            var gameStateBegin = new GameState(gameState).ResetLinesAndScore();

            Node root = new Node(gameStateBegin);
            Node goal = null;
            var bestScore = double.NegativeInfinity;

            foreach (var successor in root.GetSuccessors())
            {
                var score = Score(successor);
                if (score > bestScore)
                {
                    goal = successor;
                    goal.Score = score;
                    bestScore = score;
                }
            }

            if (goal?.Parent != null)
            {
                var result = new SearchResult
                {
                    CurrentGameState = root.GameState,
                    GoalGameState = goal.GameState,
                    Way = goal.Way
                };
                result.Moves = GetMoves(result.Way);
                return result;
            }

            // game is in the wrong state
            // no search is possible here
            return null;
        }
    }
}
