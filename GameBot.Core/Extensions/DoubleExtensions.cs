using System;

namespace GameBot.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static TimeSpan ToTimestamp(this double seconds)
        {
            var wholeSeconds = (int)seconds;
            var milliseconds = (int)((seconds - wholeSeconds) * 1000);
            return new TimeSpan(0, 0, 0, wholeSeconds, milliseconds);
        }

        public static double Clamp(this double value, double lower, double upper)
        {
            if (value > upper) return upper;
            if (value < lower) return lower;
            return value;
        }
    }
}
