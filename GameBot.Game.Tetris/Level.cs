using GameBot.Game.Tetris.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Game.Tetris
{
    /// <summary>
    /// In Marathon (called A-TYPE), when the player line clear (startLevel * 10 + 10),
    /// the level advances by 1. After this, the level advances by 1 for every 10 lines
    /// http://harddrop.com/wiki/Tetris_(Game_Boy)
    /// </summary>
    public class Level
    {
        private static double Framerate = 59.73;
        private static int[] LevelSpeeds = new[] { 53, 49, 45, 41, 37, 33, 28, 22, 17, 11, 10, 9, 8, 7, 6, 6, 5, 5, 4, 4, 3 };

        public static int GetLevel(GameType gameType, int startLevel, int clearedLines)
        {
            if (gameType == GameType.AType)
            {
                GetLevelAType(startLevel, clearedLines);
            }
            throw new NotImplementedException("This game type is not implemented.");
        }

        public static int GetLevelAType(int startLevel, int clearedLines)
        {
            int clearedLinesAfterStartLevel = clearedLines - (startLevel * 10 + 10);
            if (clearedLinesAfterStartLevel >= 0)
            {
                return startLevel + clearedLinesAfterStartLevel / 10;
            }
            return startLevel;
        }

        // Frames per row
        public static int GetSpeed(int level)
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
    }
}
