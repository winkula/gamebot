﻿using System.Collections.Generic;

namespace GameBot.Core
{
    /// <summary>
    /// Represents a configuration repository.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Reads a value from the configuration repository. Throws an exception if the key is not defined.
        /// </summary>
        /// <typeparam name="T">Type of the configuration value.</typeparam>
        /// <param name="key">Key of the configuration value.</param>
        /// <returns>The configuration value belonging to the specified key.</returns>
        T Read<T>(string key);

        /// <summary>
        /// Reads a value from the configuration repository. Returns a default value if the key is not defined.
        /// </summary>
        /// <typeparam name="T">Type of the configuration value.</typeparam>
        /// <param name="key">Key of the configuration value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The configuration value belonging to the specified key.</returns>
        T Read<T>(string key, T defaultValue);

        /// <summary>
        /// Writes a value to the configuration repository.
        /// </summary>
        /// <typeparam name="T">Type of the configuration value.</typeparam>
        /// <param name="key">Key of the configuration value.</param>
        /// <param name="value">The value.</param>
        void Write<T>(string key, T value);

        /// <summary>
        /// Reads a collection of values from the configuration repository. Throws an exception if the key is not defined.
        /// </summary>
        /// <typeparam name="T">Type of the configuration value.</typeparam>
        /// <param name="key">Key of the configuration value.</param>
        /// <returns>The configuration values belonging to the specified key.</returns>
        IEnumerable<T> ReadCollection<T>(string key);

        /// <summary>
        /// Reads a collection of values from the configuration repository. Returns a default value if the key is not defined.
        /// </summary>
        /// <typeparam name="T">Type of the configuration value.</typeparam>
        /// <param name="key">Key of the configuration value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The configuration values belonging to the specified key.</returns>
        IEnumerable<T> ReadCollection<T>(string key, IEnumerable<T> defaultValue);

        /// <summary>
        /// Writes a collection of values to the configuration repository.
        /// </summary>
        /// <typeparam name="T">Type of the configuration values.</typeparam>
        /// <param name="key">Key of the configuration values.</param>
        /// <param name="values">The collection of values.</param>
        void WriteCollection<T>(string key, IEnumerable<T> values);

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        void Save();
    }
}
