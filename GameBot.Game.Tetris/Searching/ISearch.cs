using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Searching
{
    public interface ISearch
    {
        SearchResult Search(GameState gameState);
    }
}
