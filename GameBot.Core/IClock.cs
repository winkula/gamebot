using System;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a provider for the relative time since the GameBot engine has been started.
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// Starts the measurement of the relative time.
        /// </summary>
        void Start();

        /// <summary>
        /// Sleeps for e specified amount of time.
        /// </summary>
        /// <param name="miliseconds">Time in miliseconds to sleep.</param>
        void Sleep(int miliseconds);

        /// <summary>
        /// Sleeps for e specified amount of time.
        /// </summary>
        /// <param name="timeSpan">Time to sleep as TimeSpan.</param>
        void Sleep(TimeSpan timeSpan);

        /// <summary>
        /// Gets the relative time since the GameBot engine has been started.
        /// </summary>
        TimeSpan Time { get; }
    }
}
