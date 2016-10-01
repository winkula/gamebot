using GameBot.Core.Data;

namespace GameBot.Core
{
    /// <summary>
    /// Represents an engine that can run the GameBot.
    /// </summary>
    public interface IEngine
    {
        // TODO: remove
        /// <summary>
        /// Runs the engine in a loop.
        /// This method is intended to call only once in the beginning of the application.
        /// </summary>
        void Run();
        
        /// <summary>
        /// Initializes the engine. Must be called before any calls to 'Run'.
        /// This method is intended to call before a loop.
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Runs one step of the engine and returns the result.
        /// This method is intended to call in a loop.
        /// </summary>
        /// <param name="play">If the agent should play.</param>
        /// <returns>The engines result.</returns>
        EngineResult Step(bool play);
    }
}
