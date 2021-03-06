﻿using Emgu.CV;
using System;

namespace GameBot.Core
{
    /// <summary>
    /// Represents an engine that can run the GameBot.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Tells the engine if the internal Agent should be called in the next step.
        /// Default is false.
        /// </summary>
        bool Play { get; set; }

        /// <summary>
        /// Initializes the engine. Must be called before any calls to 'Step'.
        /// This method is intended to call before a loop.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Runs one step of the engine and returns the result.
        /// This method is intended to call in a loop.
        /// </summary>
        /// <param name="showImage">Callback with the original image.</param>
        /// <param name="showProcessedImage">Callback with the processed image.</param>
        /// <returns>The engines result.</returns>
        void Step(Action<Mat> showImage = null, Action<Mat> showProcessedImage = null);
        
        /// <summary>
        /// Sends a message to the underlying agent.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void Send(string message);
    }
}
