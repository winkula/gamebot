using System;
using GameBot.Core.Data;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a handler for commands to the Game Boy device.
    /// </summary>
    public interface IExecutor
    {
        /// <summary>
        /// Hits a button for a minimal amount of time.
        /// This blocks the calling thread.
        /// </summary>
        /// <param name="button">The button to hit.</param>
        void Hit(Button button);

        /// <summary>
        /// Hits a button for a specifified amount of time.
        /// This blocks the calling thread.
        /// </summary>
        /// <param name="button">The button to hit.</param>
        /// <param name="duration">The button to hit.</param>
        void Hit(Button button, TimeSpan duration);

        /// <summary>
        /// Hits a button for a minimal amount of time.
        /// This call is async and doesn't block the calling thread.
        /// </summary>
        /// <param name="button">The button to hit.</param>
        void HitAsync(Button button);

        /// <summary>
        /// Hits a button for a specifified amount of time.
        /// This call is async and doesn't block the calling thread.
        /// </summary>
        /// <param name="button">The button to hit.</param>
        /// <param name="duration">The button to hit.</param>
        void HitAsync(Button button, TimeSpan duration);

        /// <summary>
        /// Presses a button down.
        /// </summary>
        /// <param name="button">The button to press.</param>
        void Press(Button button);

        /// <summary>
        /// Releases a button.
        /// </summary>
        /// <param name="button">The button to release.</param>
        void Release(Button button);
    }
}
