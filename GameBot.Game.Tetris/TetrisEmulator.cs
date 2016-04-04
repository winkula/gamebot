using GameBot.Core;
using System;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris
{
    public class TetrisEmulator : IEmulator<TetrisGameState>
    {
        private readonly Random random = new Random();

        public TetrisEmulator()
        {
            GameState = new TetrisGameState();
        }

        public TetrisGameState GameState { get; }
        
        public void Execute(ICommand command)
        {
            switch (command.Button)
            {
                case Button.Down:
                    if (command.Duration > TimeSpan.Zero)
                    {
                        GameState.Drop();
                    }
                    else
                    {
                        GameState.Fall();
                    }
                    break;

                case Button.Left:
                    GameState.Piece.Left();
                    break;

                case Button.Right:
                    GameState.Piece.Right();
                    break;

                case Button.A:
                    GameState.Piece.Rotate();
                    break;

                case Button.B:
                    GameState.Piece.RotateCounterclockwise();
                    break;

                default:
                    break;
            }
        }
        
        public override string ToString()
        {
            return GameState.ToString();
        }
    }
}
