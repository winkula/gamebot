using GameBot.Core.Data;
using GameBot.Engine.Physical.Actuators;
using NUnit.Framework;
using System.Threading;
using GameBot.Core.Configuration;

namespace GameBot.Test.Engine.Physical.Actuators
{
    [TestFixture]
    public class PhysicalActuatorTests
    {
        [Test]
        public void ConstructDispose()
        {
            using (var actuator = new PhysicalActuator(new AppSettingsConfig()))
            {
            }
        }

        [Test]
        public void TestAll()
        {
            using (var actuator = new PhysicalActuator(new AppSettingsConfig()))
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
}
