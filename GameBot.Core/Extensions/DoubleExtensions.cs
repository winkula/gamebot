namespace GameBot.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static double Clamp(this double value, double lower, double upper)
        {
            if (value > upper) return upper;
            if (value < lower) return lower;
            return value;
        }
    }
}
