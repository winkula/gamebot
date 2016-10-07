using GameBot.Core;
using System;
using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris
{
    public class TetrisSimulator : ISimulator<GameState>, IActuator
    {
        private readonly Random random = new Random();

        public TetrisSimulator()
        {
            GameState = new GameState();
        }

        public GameState GameState { get; }

        public void Simulate(ICommand command)
        {
            command.Execute(this);
        }

        public void Hit(Button button)
        {
            switch (button)
            {
                case Button.Left: GameState.Left(); break;
                case Button.Right: GameState.Right(); break;
                case Button.A: GameState.Rotate(); break;
                case Button.B: GameState.RotateCounterclockwise(); break;
                case Button.Down: GameState.Fall(); break;
                default: break;
            }
        }

        public void Press(Button button)
        {
            if (button == Button.Down)
            {
                GameState.Drop();
            }
        }

        public void Release(Button button)
        {
            // ignore
        }

        public override string ToString()
        {
            return GameState.ToString();
        }
    }
}
