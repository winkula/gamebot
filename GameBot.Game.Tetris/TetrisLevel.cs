using System;

namespace GameBot.Game.Tetris
{
    /// <summary>
    /// In Marathon (called A-TYPE), when the player line clear (startLevel * 10 + 10),
    /// the level advances by 1. After this, the level advances by 1 for every 10 lines
    /// http://harddrop.com/wiki/Tetris_(Game_Boy)
    /// </summary>
    public static class TetrisLevel
    {
        // frames per row
        private static readonly int[] _levelSpeeds = { 53, 49, 45, 41, 37, 33, 28, 22, 17, 11, 10, 9, 8, 7, 6, 6, 5, 5, 4, 4, 3 };

        // For A Type
        public static int GetLevel(int startLevel, int clearedLines)
        {
            // TODO: add checks

            var limit = (startLevel + 1) * 10;
            if (clearedLines < limit) return startLevel;
            return Math.Min(20, clearedLines / 10);
        }

        // Frames per row
        public static int GetFramesPerRow(int level)
        {
            if (level < 0) throw new ArgumentException("Level must not be negative");
            if (level > 20) throw new ArgumentException("Level can't be bigger than 20");

            return _levelSpeeds[level];
        }

        public static TimeSpan GetDuration(int level, int rows)
        {
            if (level < 0) throw new ArgumentException("Level must not be negative");
            if (level > 20) throw new ArgumentException("Level can't be bigger than 20");

            return TimeSpan.FromSeconds(rows * _levelSpeeds[level] / TetrisTiming.Framerate);
        }

        // how many rows will a tile maximal fall in a specific time span?
        public static int GetFallDistance(int level, TimeSpan duration)
        {
            if (level < 0) throw new ArgumentException("Level must not be negative");
            if (level > 20) throw new ArgumentException("Level can't be bigger than 20");

            double frames = duration.TotalSeconds * TetrisTiming.Framerate;
            int framePerRow = GetFramesPerRow(level);

            return (int)Math.Ceiling(frames / framePerRow);
        }
    }
}
