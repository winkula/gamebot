using NUnit.Framework;
using System;
using System.Threading;
using Tinkerforge;

namespace GameBot.Test.Misc
{
    [Ignore]
    [TestFixture]
    public class TinkerforgeTests
    {
        private string _host = "localhost";
        private int _port = 4223;
        private string _uidMaster = "6JKbWn";
        private string _uidRelay1 = "r5F";
        private string _uidRelay2 = "mTC";
        private string _uidTemp = "qBx";

        [Test]
        public void TestConnection()
        {
            IPConnection ipcon = new IPConnection(); // Create IP connection
            BrickMaster master = new BrickMaster(_uidMaster, ipcon); // Create device object
            BrickletIndustrialQuadRelay or1 = new BrickletIndustrialQuadRelay(_uidRelay1, ipcon);
            BrickletIndustrialQuadRelay or2 = new BrickletIndustrialQuadRelay(_uidRelay2, ipcon);
            BrickletTemperatureIR tir = new BrickletTemperatureIR(_uidTemp, ipcon);

            ipcon.Connect(_host, _port); // Connect to brickd
                                       // Don't use device before ipcon is connected

            // Get current stack voltage (unit is mV)
            int stackVoltage = master.GetStackVoltage();
            Console.WriteLine("Stack Voltage: " + stackVoltage / 1000.0 + " V");

            // Get current stack current (unit is mA)
            int stackCurrent = master.GetStackCurrent();
            Console.WriteLine("Stack Current: " + stackCurrent / 1000.0 + " A");

            short chipTemp = tir.GetAmbientTemperature();
            Console.WriteLine("Chibi master address: " + chipTemp / 10 + "°/C");

            int delay = 50;

            while (true)
            {
                ConsoleKeyInfo insertKey = Console.ReadKey();
                int result = insertKey.KeyChar;
                // UP
                if (result.Equals(32)){
                    or1.SetValue(1 << 0);
                    Thread.Sleep(delay);
                    or1.SetValue(1 << 0);
                    Thread.Sleep(delay);
                } 
                // DOWN
                if (result.Equals(32))
                {
                    or1.SetValue(1 << 0);
                    Thread.Sleep(delay);
                    or1.SetValue(1 << 0);
                    Thread.Sleep(delay);
                }
                // LEFT
                if (result.Equals(97))
                {
                    or1.SetValue(1 << 0);
                    Thread.Sleep(delay);
                    or1.SetValue(1 << 0);
                    Thread.Sleep(delay);
                }
                // RIGHT
                if (result.Equals(32))
                {
                    or1.SetValue(1 << 0);
                    Thread.Sleep(delay);
                    or1.SetValue(1 << 0);
                    Thread.Sleep(delay);
                }
                // START
                if (result.Equals(115))
                {
                    or2.SetValue(1);
                    Thread.Sleep(delay);
                    or2.SetValue(0);
                    Thread.Sleep(delay);
                }
                // SELECT
                if (result.Equals(83))
                {
                    or2.SetValue(4);
                    Thread.Sleep(delay);
                    or2.SetValue(0);
                    Thread.Sleep(delay);
                }
                // A
                if (result.Equals(97))
                {
                    or2.SetValue(2);
                    Thread.Sleep(delay);
                    or2.SetValue(0);
                    Thread.Sleep(delay);
                }
                // B
                if (result.Equals(98))
                {
                    or2.SetValue(8);
                    Thread.Sleep(delay);
                    or2.SetValue(0);
                    Thread.Sleep(delay);
                }
                Console.WriteLine(result);
                // ESC for EXIT
                if (result.Equals(27))
                {
                    break;
                }


            }
            ipcon.Disconnect();
        }
    }
}
