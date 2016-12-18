using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Simulators
{
    public class TetrisSimulator
    {
        public GameState GameState { get; }

        public TetrisSimulator(GameState gameState = null)
        {
            var board = new Board();
            var piece = new Piece();
            var nextPiece = Tetriminos.GetRandom();

            GameState = gameState ?? new GameState(board, piece, nextPiece) { StartLevel = 9 };
        }

        public void Simulate(Move move)
        {
            switch (move)
            {
                case Move.Left: GameState.Left(); break;
                case Move.Right: GameState.Right(); break;
                case Move.Rotate: GameState.Rotate(); break;
                case Move.RotateCounterclockwise: GameState.RotateCounterclockwise(); break;
                case Move.Fall: GameState.Fall(); break;
                case Move.Drop: GameState.Drop(); break;
            }
        }

        public override string ToString()
        {
            return GameState.ToString();
        }
    }
}
