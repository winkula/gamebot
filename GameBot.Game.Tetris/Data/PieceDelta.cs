using System;

namespace GameBot.Game.Tetris.Data
{
    public class PieceDelta
    {
        public int Orientation { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public PieceDelta(Piece current, Piece target)
        {
            if (current == null) throw new ArgumentNullException("current");
            if (target == null) throw new ArgumentNullException("target");

            Orientation = target.Orientation - current.Orientation;
            X = target.X - current.X;
            Y = target.Y - current.Y;
        }
    }
}
