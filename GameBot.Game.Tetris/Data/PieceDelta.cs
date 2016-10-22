using System;

namespace GameBot.Game.Tetris.Data
{
    public class PieceDelta
    {
        public int Orientation { get; }
        public int X { get; }
        public int Y { get; private set; }

        /// <summary>
        /// Current piece is in spawn position, if orientation and x coordinate are equals.
        /// The y coordinate (fall distance) may be different.
        /// </summary>
        public bool IsUntouched => Orientation == 0 && X == 0;
        
        public PieceDelta(Piece current, Piece target)
        {
            if (current == null) throw new ArgumentNullException(nameof(current));
            if (target == null) throw new ArgumentNullException(nameof(target));

            Orientation = target.Orientation - current.Orientation;
            X = target.X - current.X;
            Y = target.Y - current.Y;
        }
    }
}
