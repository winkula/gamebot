using System;
using GameBot.Core.Extensions;

namespace GameBot.Game.Tetris
{
    public static class Timing
    {
        // this should be in sync with the duration after a button was pressed/released
        // important is the blocking time until the control goes back in the code
        private static readonly TimeSpan _delayHold = TimeSpan.FromMilliseconds(35);
        private static readonly TimeSpan _delayRelease = TimeSpan.FromMilliseconds(40);

        public static TimeSpan AnalyzeFallDurationPaddingTime = TimeSpan.FromMilliseconds(50);
        public static TimeSpan DropDurationPaddingTime = TimeSpan.FromMilliseconds(50);

        public static TimeSpan CheckFallDurationPaddingTime = TimeSpan.FromMilliseconds(50) + _delayRelease;

        public static TimeSpan GetExecutionDuration(int commands)
        {
            // the drop command is not counted here, because
            // it has no delay time (press and release, no hit)
            commands = Math.Max(0, commands - 1);

            return (_delayHold + _delayRelease).Multiply(commands);
        }
    }
}
