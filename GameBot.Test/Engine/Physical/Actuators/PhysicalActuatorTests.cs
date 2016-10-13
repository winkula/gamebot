using GameBot.Core.Data;
using GameBot.Engine.Physical.Actuators;
using NUnit.Framework;
using System.Threading;
using System;
using System.Collections.Generic;
using GameBot.Core;

namespace GameBot.Test.Engine.Physical.Actuators
{
    [TestFixture]
    public class PhysicalActuatorTests
    {
        [Test]
        public void ConstructDispose()
        {
            using (var actuator = new PhysicalActuator(new MockConfig()))
            {
            }
        }

        [Test]
        public void TestAll()
        {
            using (var actuator = new PhysicalActuator(new MockConfig()))
            {
                actuator.Hit(Button.Up);
                Thread.Sleep(1000);

                actuator.Hit(Button.Down);
                Thread.Sleep(1000);

                actuator.Hit(Button.Left);
                Thread.Sleep(1000);

                actuator.Hit(Button.Right);
                Thread.Sleep(1000);

                actuator.Hit(Button.A);
                Thread.Sleep(1000);

                actuator.Hit(Button.B);
                Thread.Sleep(1000);

                actuator.Hit(Button.Start);
                Thread.Sleep(1000);

                actuator.Hit(Button.Select);
                Thread.Sleep(1000);
            }
        }
    }

    // TODO: use moq?
    internal class MockConfig : IConfig
    {
        public T Read<T>(string key)
        {
            throw new NotImplementedException();
        }

        public T Read<T>(string key, T defaultValue)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ReadCollection<T>(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ReadCollection<T>(string key, IEnumerable<T> defaultValue)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Write<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public void WriteCollection<T>(string key, IEnumerable<T> values)
        {
            throw new NotImplementedException();
        }
    }
}
