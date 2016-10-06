using GameBot.Game.Tetris.Data;
using System.Collections.Generic;

namespace GameBot.Game.Tetris.Searching
{
    public class SearchResult
    {
        public TetrisGameState CurrentGameState { get; set; }
        public TetrisGameState GoalGameState { get; set; }
        public Way Way { get; set; }
        public IEnumerable<Move> Moves { get; set; }
    }
}
