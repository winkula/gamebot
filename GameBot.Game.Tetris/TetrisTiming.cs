﻿using System;

namespace GameBot.Game.Tetris
{
    /// <summary>
    /// Source: http://harddrop.com/wiki/Tetris_(Game_Boy)
    /// </summary>
    public static class TetrisTiming
    {
        public const double Framerate = 59.73;
        public static TimeSpan LineClearDuration => GetDurationFormFrames(91);
        private static TimeSpan EntryDelayDuration => GetDurationFormFrames(2);

        public static TimeSpan GetDropDuration(int rows)
        {
            // the speed of the drop is the same as the normal speed in level 20
            // this has been approved experimentally
            return TetrisLevel.GetDuration(20, rows) + EntryDelayDuration;
        }

        private static TimeSpan GetDurationFormFrames(int frames)
        {
            return TimeSpan.FromSeconds(frames / Framerate);
        }
    }
}
