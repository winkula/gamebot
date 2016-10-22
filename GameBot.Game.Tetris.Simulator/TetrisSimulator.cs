using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Simulator
{
    public class TetrisSimulator
    {
        public TetrisSimulator()
        {
            GameState = new GameState(new Piece(), Tetriminos.GetRandom());
        }

        public GameState GameState { get; }
        
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
