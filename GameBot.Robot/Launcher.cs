﻿using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Robot.Actuators;
using System.Threading;

namespace GameBot.Robot
{
    public class Launcher
    {
        static void Main(string[] args)
        {
            //RunSimulations();
            RunMain();
            //TestTinkerfoge();
        }

        static void TestTinkerfoge()
        {
            using (var actuator = new Actuator())
            {
                actuator.Hit(Button.Up);
                //actuator.Press(Button.B);
                //Thread.Sleep(1000);
                //actuator.Release(Button.Up);
            }
        }

        static void RunMain()
        {
            using (var container = Bootstrapper.GetInitializedContainer())
            {
                container.GetInstance<IEngine>().Run();
            }
        }

        static void RunSimulations()
        {
            for (int i = 0; i < 20; i++)
            {
                var container = Bootstrapper.GetInitializedContainer(Bootstrapper.EngineType.Fast);
                container.GetInstance<IEngine>().Run();
            }
        }
    }
}
