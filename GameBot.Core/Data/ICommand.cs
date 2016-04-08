﻿using System;

namespace GameBot.Core.Data
{
    /// <summary>
    /// Represents a command to the Game Boy device, normally a button press.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the button, that must be pressed.
        /// </summary>
        Button Button { get; }

        /// <summary>
        /// Gets the moment in time, when the button must be pressed.
        /// </summary>
        TimeSpan? Press { get; }

        /// <summary>
        /// Gets the moment in time, when the button must be released.
        /// </summary>
        TimeSpan? Release { get; }
    }
}
