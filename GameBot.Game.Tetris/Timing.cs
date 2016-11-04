using System;

namespace GameBot.Game.Tetris
{
    // TODO: finetune!
    public class Timing
    {
        // this should be in sync with the duration after a button was pressed/released
        // important is the blocking time until the control goes back in the code
        private static readonly TimeSpan _sleepAfterButtonTime = TimeSpan.FromMilliseconds(80); // 500

        public static TimeSpan AnalyzeFallDurationPaddingTime = TimeSpan.FromMilliseconds(500) + _sleepAfterButtonTime; // 30?
        public static TimeSpan CheckFallDurationPaddingTime = TimeSpan.FromMilliseconds(30) + _sleepAfterButtonTime; // 30?
        public static TimeSpan DropDurationPaddingTime = TimeSpan.FromMilliseconds(60); // 30?
    }
}
