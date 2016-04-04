﻿using GameBot.Core.Data;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a robotic arm that can press buttons on a Game Boy device.
    /// </summary>
    public interface IActor
    {
        /// <summary>
        /// Hits a button for a minimal amount of time.
        /// </summary>
        /// <param name="button">The button to hit.</param>
        void Hit(Button button);

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