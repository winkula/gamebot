using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Robot.Actuators;
using GameBot.Robot.Configuration;
using System.Threading;

namespace GameBot.Robot
{
    public class Program
    {
        static void Main(string[] args)
        {
            //RunSimulations();
            RunMain();
            //TestTinkerfoge();
        }

        static void TestTinkerfoge()
        {
            using (var actuator = new Actuator(new Config()))
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
            }
        }

        static void RunSimulations()
        {
            for (int i = 0; i < 20; i++)
            {
                var container = Bootstrapper.GetInitializedContainer(Engines.EngineMode.Fast);
                container.GetInstance<IEngine>().Step();
            }
        }
    }
}
