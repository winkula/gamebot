using System.Collections.Generic;
using System.Linq;
using GameBot.Core;
using Moq;

namespace GameBot.Test.Extensions
{
    public static class MockExtensions
    {
        public static void ConfigValue<TValue>(this Mock<IConfig> mock, string key, TValue value)
        {
            mock.Setup(x => x.Read<TValue>(key)).Returns(value);
            mock.Setup(x => x.Read(key, It.IsAny<TValue>())).Returns(value);
        }
        
        public static void ConfigCollection<TValue>(this Mock<IConfig> mock, string key, IEnumerable<TValue> value)
        {
            var list = value.ToList();
            mock.Setup(x => x.ReadCollection<TValue>(key)).Returns(list);
            mock.Setup(x => x.ReadCollection(key, It.IsAny<IEnumerable<TValue>>())).Returns(list);
        }
    }
}
