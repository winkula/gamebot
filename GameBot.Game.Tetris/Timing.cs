using System;

namespace GameBot.Game.Tetris
{
    // TODO: finetune!
    public class Timing
    {
        // this should be in sync with the duration after a butten was pressed/released
        // important is the blocking time until the control goes back in the code
        public static TimeSpan SleepAfterButtonTime = TimeSpan.FromMilliseconds(0); // 500

        public static TimeSpan AnalyzeFallDurationPaddingTime = TimeSpan.FromMilliseconds(1) + SleepAfterButtonTime; // 30?
        public static TimeSpan CheckFallDurationPaddingTime = TimeSpan.FromMilliseconds(1) + SleepAfterButtonTime; // 30?
        public static TimeSpan DropDurationPaddingTime = TimeSpan.FromMilliseconds(1); // 30?
    }
}
