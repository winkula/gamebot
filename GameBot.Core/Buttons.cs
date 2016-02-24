using System;

namespace GameBot.Core
{
    [Flags]
    public enum Buttons
    {
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        A = 16,
        B = 32,
        Start = 64,
        Select = 128
    }
}
