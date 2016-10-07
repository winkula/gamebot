﻿using System;

namespace GameBot.Game.Tetris
{
    /// <summary>
    /// In Marathon (called A-TYPE), when the player line clear (startLevel * 10 + 10),
    /// the level advances by 1. After this, the level advances by 1 for every 10 lines
    /// http://harddrop.com/wiki/Tetris_(Game_Boy)
    /// </summary>
    public class TetrisLevel
    {
        private static double Framerate = 59.73;

        // frames per row
        private static int[] LevelSpeeds = new[] { 53, 49, 45, 41, 37, 33, 28, 22, 17, 11, 10, 9, 8, 7, 6, 6, 5, 5, 4, 4, 3 };

        // For A Type
        public static int GetLevel(int startLevel, int clearedLines)
        {
            if (clearedLines < 10) return startLevel;
            return Math.Min(20, startLevel + clearedLines / 10);
        }

        // Frames per row
        public static int GetFramesPerRow(int level)
        {
            if (level < 0) throw new ArgumentException("Level must not be negative");

            if (level > 20) level = 20;
            return LevelSpeeds[level];
        }

        public static TimeSpan GetDuration(int level, int rows)
        {
            if (level < 0) throw new ArgumentException("Level must not be negative");

            if (level > 20) level = 20;
            return TimeSpan.FromSeconds(rows * LevelSpeeds[level] / Framerate);
        }

        public static TimeSpan GetFreeFallDuration(int rows)
        {
            // this is just an estimate
            // the speed of the freefall is not documented anywhere
            return GetDuration(20, rows);
        }

        // how many rows will a tile maximal fall in a specific time span?
        public static int GetMaxFallDistance(int level, TimeSpan duration)
        {
            int frames = (int)(duration.TotalSeconds * Framerate);
            int framePerRow = GetFramesPerRow(level);

            // we can always miss the next frame, no matter how fast we are
            // so we always add 1
            return frames / framePerRow + 1;
        }
    }
}
