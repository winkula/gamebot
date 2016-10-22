using System;

namespace GameBot.Game.Tetris
{
    public static class TetrisTiming
    {
        // source: http://harddrop.com/wiki/Tetris_(Game_Boy)
        public const double Framerate = 59.73;
        
        // this is just an estimate (experimentally verified)
        public static TimeSpan LineRemovingDuration => GetDurationFormFrames(92);

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
    }
}
