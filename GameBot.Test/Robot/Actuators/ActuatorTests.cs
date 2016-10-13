﻿using GameBot.Core.Data;
using GameBot.Robot.Actuators;
using GameBot.Robot.Configuration;
using NUnit.Framework;
using System.Threading;

namespace GameBot.Test.Robot.Actuators
{
    [TestFixture]
    public class ActuatorTests
    {
        [Test]
        public void ConstructDispose()
        {
            using (var actuator = new Actuator(new Config()))
            {
            }
        }

        [Test]
        public void TestAll()
        {
            using (var actuator = new Actuator(new Config()))
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