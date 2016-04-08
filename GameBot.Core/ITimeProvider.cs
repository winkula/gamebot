using System;

namespace GameBot.Core
{
    public interface ITimeProvider
    {
        void Start();

        TimeSpan Time { get; }
    }
}
