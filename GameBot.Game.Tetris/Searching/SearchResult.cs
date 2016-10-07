using GameBot.Game.Tetris.Data;
using System.Collections.Generic;

namespace GameBot.Game.Tetris.Searching
{
    public class SearchResult
    {
        public GameState CurrentGameState { get; set; }
        public GameState GoalGameState { get; set; }
        public Way Way { get; set; }
        public IEnumerable<Move> Moves { get; set; }
    }
}
