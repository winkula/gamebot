using System;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris
{
    public class TetrisSimulator
    {
        private readonly Random _random = new Random();

        public TetrisSimulator()
        {
            GameState = new GameState();
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
                default: break;
            }
        }

        public override string ToString()
        {
            return GameState.ToString();
        }
    }
}
