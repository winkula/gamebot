using GameBot.Core.Data;
using GameBot.Game.Tetris.Data;
using System;
using System.Collections.Generic;

namespace GameBot.Game.Tetris
{
    public class TetrisGameStateFull : AbstractGameState
    {
        public TetrisScreen Screen { get; set; }
        public TetrisGameState State { get; set; }
        public int? Player { get; set; }
        public TetrisGameType? GameType { get; set; }
        public int? MusicType { get; set; }
        public int? Level { get; set; }
        public int? High { get; set; }
        public int? Score { get; set; }
        public bool? IsPause { get; set; }

        public TetrisGameStateFull()
        {
            State = new TetrisGameState();
        }

        public override string ToString()
        {
            return State.ToString();
        }
    }
}
