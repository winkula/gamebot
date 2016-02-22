using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Core
{
    [Flags]
    public enum Commands
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
