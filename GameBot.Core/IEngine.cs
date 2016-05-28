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

        void Configure(string key, object value);
    }
}
