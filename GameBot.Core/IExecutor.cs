using System;
using System.Collections.Generic;
using GameBot.Core.Data;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a handler for commands to the Game Boy device.
    /// </summary>
    public interface IExecutor
    {
        /// <summary>
        /// Hits a button.
        /// </summary>
        /// <param name="button">The button to hit.</param>
        void Hit(Button button);

        /// <summary>
        /// Hits buttons parallel.
        /// </summary>
        /// <param name="buttons">The buttons to hit.</param>
        void Hit(IEnumerable<Button> buttons);

        /// <summary>
        /// Hits a button and waits for the specified duration.
        /// </summary>
        /// <param name="button">The button to hit.</param>
        /// <param name="duration">The wait duration.</param>
        void HitWait(Button button, TimeSpan duration);

        /// <summary>
        /// Holds a button for a specifified amount of time.
        /// </summary>
        /// <param name="button">The button to hold.</param>
        /// <param name="duration">The hold duration.</param>
        void Hold(Button button, TimeSpan duration);

        /// <summary>
        /// Hits a button.
        /// This call is async and doesn't block the calling thread.
        /// </summary>
        /// <param name="button">The button to hit.</param>
        void HitAsync(Button button);

        /// <summary>
        /// Holds a button for a specifified amount of time.
        /// This call is async and doesn't block the calling thread.
        /// </summary>
        /// <param name="button">The button to hold.</param>
        /// <param name="duration">The hold duration.</param>
        void HoldAsync(Button button, TimeSpan duration);

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
