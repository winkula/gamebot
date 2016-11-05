using System;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris.Data
{
    public enum Move
    {
        Left = 0,
        Right = 1,

        Rotate = 2,
        RotateCounterclockwise = 3,

        Fall = 4,

        Drop = 5
    }

    public static class MoveExtensions
    {
        private static readonly Action<GameState>[] _actions =
        {
            g => { g.Left(); }, // Left
            g => { g.Right(); }, // Right
            g => { g.Rotate(); }, // Rotate
            g => { g.RotateCounterclockwise(); }, // RotateCounterclockwise
            g => { g.Fall(); }, // Fall
            g => { g.Drop(); } // Drop
        };

        private static readonly Button[] _buttons =
        {
            Button.Left, // Left
            Button.Right, // Right
            Button.A, // Rotate
            Button.B, // RotateCounterclockwise
            Button.Down, // Fall
            Button.Down // Drop
        };

        public static void Apply(this Move move, GameState currentGameState)
        {
            _actions[(int)move](currentGameState);
        }

        public static void Check(this Move move, GameState currentGameState)
        {
            _actions[(int)move](new GameState(currentGameState));
        }

        public static Button ToButton(this Move move)
        {
            return _buttons[(int)move];
        }
    }
}
