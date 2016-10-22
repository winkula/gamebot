using System;

namespace GameBot.Core.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan ToTimestamp(this double seconds)
        {
            var wholeSeconds = (int)seconds;
            var milliseconds = (int)((seconds - wholeSeconds) * 1000);
            return new TimeSpan(0, 0, 0, wholeSeconds, milliseconds);
        }
    }
}
