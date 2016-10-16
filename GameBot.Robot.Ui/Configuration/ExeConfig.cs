using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;

namespace GameBot.Robot.Ui.Configuration
{
    public class ExeConfig : IConfig
    {
        private const char _delimiter = ',';
        private readonly System.Configuration.Configuration _configuration;

        public ExeConfig()
        {
            _configuration = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
        }

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
            _configuration.AppSettings.Settings[key].Value = value.ToString();
        }

        public IEnumerable<T> ReadCollection<T>(string key)
        {
            string values = ConfigurationManager.AppSettings[key];
            if (values == null) throw new ArgumentException($"config value with key {key} not found.");

            foreach (var value in values.Split(_delimiter))
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

            foreach (var value in values.Split(_delimiter))
            {
                yield return Get<T>(value);
            }
        }

        public void WriteCollection<T>(string key, IEnumerable<T> values)
        {
            _configuration.AppSettings.Settings[key].Value = string.Join(",", values);
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

        public void Save()
        {
            _configuration.Save(ConfigurationSaveMode.Modified);
        }
    }
}
