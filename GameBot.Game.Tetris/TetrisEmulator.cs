using GameBot.Core;
using System;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris
{
    public class TetrisEmulator : IEmulator<TetrisGameStateFull>
    {
        private readonly Random random = new Random();

        public TetrisEmulator()
        {
            GameState = new TetrisGameStateFull();
        }

        public TetrisGameStateFull GameState { get; }
        
        public void Execute(ICommand command)
        {
            switch (command.Button)
            {
                case Button.Down:
                    if (command.Duration > TimeSpan.Zero)
                    {
                        GameState.State.Drop();
                        GameState.State.RemoveLines();
                    }
                    else
                    {
                        GameState.State.Fall();
                    }
                    break;

                case Button.Left:
                    GameState.State.Piece.Left();
                    break;

                case Button.Right:
                    GameState.State.Piece.Right();
                    break;

                case Button.A:
                    GameState.State.Piece.Rotate();
                    break;

                case Button.B:
                    GameState.State.Piece.RotateCounterclockwise();
                    break;

                default:
                    break;
            }
        }
        
        public override string ToString()
        {
            return GameState.State.ToString();
        }
    }
}
