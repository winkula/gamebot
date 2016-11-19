using System;

namespace GameBot.Core.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Multiply(this TimeSpan multiplicand, int multiplier)
        {
            return TimeSpan.FromTicks(multiplicand.Ticks * multiplier);
        }
        
        public static TimeSpan Multiply(this TimeSpan multiplicand, double multiplier)
        {
            return TimeSpan.FromTicks((long)(multiplicand.Ticks * multiplier));
        }

        public static TimeSpan Clamp(this TimeSpan timeSpan, TimeSpan min, TimeSpan max)
        {
            if (timeSpan < min) return min;
            if (timeSpan > max) return max;
            return timeSpan;
        }
    }
}
