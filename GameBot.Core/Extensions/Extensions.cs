using System;

namespace GameBot.Core.Extensions
{
    public static class Extensions
    {
        public static TimeSpan ToTimestamp(this double seconds)
        {
            int wholeSeconds = (int)seconds;
            int milliseconds = (int)((seconds - wholeSeconds) * 1000);
            return new TimeSpan(0, 0, 0, wholeSeconds, milliseconds);
        }
    }
}
