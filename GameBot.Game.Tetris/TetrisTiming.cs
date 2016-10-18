using System;

namespace GameBot.Game.Tetris
{
    public static class TetrisTiming
    {
        // source: http://harddrop.com/wiki/Tetris_(Game_Boy)
        public const double Framerate = 59.73;

        public static TimeSpan GetDropDuration(int rows)
        {
            // this is just an estimate
            // the speed of the freefall is not documented anywhere
            return TetrisLevel.GetDuration(20, rows);
        }

        private static TimeSpan GetDurationFormFrames(int frames)
        {
            return TimeSpan.FromSeconds(frames / Framerate);
        }

        public static TimeSpan GetLineRemovingDuration()
        {
            // this is just an estimate
            const int framesForRemovingLines = 79;

            return GetDurationFormFrames(framesForRemovingLines);
        }
    }
}
