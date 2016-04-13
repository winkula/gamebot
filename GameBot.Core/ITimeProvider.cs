using System;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a provider for the relative time since the GameBot engine has been started.
    /// </summary>
    public interface ITimeProvider
    {
        /// <summary>
        /// Starts the measurement of the relative time.
        /// </summary>
        void Start();

        /// <summary>
        /// Gets the relative time since the GameBot engine has been started.
        /// </summary>
        TimeSpan Time { get; }
    }
}
