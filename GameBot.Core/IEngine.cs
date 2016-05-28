using GameBot.Core.Data;
using System;
using System.Linq.Expressions;

namespace GameBot.Core
{
    /// <summary>
    /// Represents an engine that can run the GameBot.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Runs the engine.
        /// </summary>
        void Run();
        
        /// <summary>
        /// Runs only one step on the engine and returns the result.
        /// </summary>
        /// <returns>The result of the step</returns>
        EngineResult Step();

        /// <summary>
        /// Configures a value on the engine.
        /// </summary>
        /// <param name="key">Config key name</param>
        /// <param name="value">Config key value</param>
        void Configure(string key, object value);
    }
}
