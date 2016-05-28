using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace GameBot.Robot.Configuration
{
    public class Config : IConfig
    {
        private const char Delimiter = ',';

        public T Read<T>(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (value == null) throw new ArgumentException($"config value with key {key} not found.");

            return Get<T>(value);
        }

        public T Read<T>(string key, T defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (value == null)
            {
                return defaultValue;
            }

            return Get<T>(value);
        }

        public void Write<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ReadCollection<T>(string key)
        {
            string values = ConfigurationManager.AppSettings[key];
            if (values == null) throw new ArgumentException($"config value with key {key} not found.");

            foreach (var value in values.Split(Delimiter))
            {
                yield return Get<T>(value);
            }
        }

        public IEnumerable<T> ReadCollection<T>(string key, IEnumerable<T> defaultValue)
        {
            string values = ConfigurationManager.AppSettings[key];
            if (values == null)
            {
                foreach (var defaultValueItem in defaultValue)
                {
                    yield return defaultValueItem;
                }
                yield break;
            }

            foreach (var value in values.Split(Delimiter))
            {
                yield return Get<T>(value);
            }
        }
        
        public void WriteCollection<T>(string key, IEnumerable<T> values)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        private T Get<T>(string value)
        {
            var type = typeof(T);
            if (type.IsEnum)
            {
                return (T)Enum.Parse(type, value);
            }
            return (T)Convert.ChangeType(value, type);
        }
    }
}
