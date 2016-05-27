using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using GameBot.Robot;
using GameBot.Robot.Actuators;
using GameBot.Robot.Executors;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;

namespace GameBot.Test.RobotTests
{
    [TestFixture]
    public class ActuatorTests
    {
        [Test]
        public void ConstructDispose()
        {
            using (var actuator = new Actuator())
            {
            }
        }

        [Test]
        public void TestAll()
        {
            using (var actuator = new Actuator())
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
