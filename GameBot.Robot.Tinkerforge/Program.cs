using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tinkerforge;

namespace GameBot.Robot.Tinkerforge
{
    class Program
    {
        private static string HOST = "localhost";
        private static int PORT = 4223;
        private static string UID_MASTER = "6JKbWn";
        private static string UID_RELAY1 = "r5F";
        private static string UID_RELAY2 = "mTC";
        private static string UID_TEMP = "qBx";
        
        static void Main()
        {
            IPConnection ipcon = new IPConnection(); // Create IP connection
            BrickMaster master = new BrickMaster(UID_MASTER, ipcon); // Create device object
            BrickletIndustrialQuadRelay or1 = new BrickletIndustrialQuadRelay(UID_RELAY1, ipcon);
            BrickletIndustrialQuadRelay or2 = new BrickletIndustrialQuadRelay(UID_RELAY2, ipcon);
            BrickletTemperatureIR tir = new BrickletTemperatureIR(UID_TEMP, ipcon);

            ipcon.Connect(HOST, PORT); // Connect to brickd
                                       // Don't use device before ipcon is connected

            // Get current stack voltage (unit is mV)
            int stackVoltage = master.GetStackVoltage();
            Console.WriteLine("Stack Voltage: " + stackVoltage / 1000.0 + " V");

            // Get current stack current (unit is mA)
            int stackCurrent = master.GetStackCurrent();
            Console.WriteLine("Stack Current: " + stackCurrent / 1000.0 + " A");

            short ChipTemp = tir.GetAmbientTemperature();
            Console.WriteLine("Chibi master address: " + ChipTemp / 10 + "°/C");

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(100);
                or1.SetValue(1 << 0);
                or2.SetValue(1 << 0);
                Thread.Sleep(100);
                or1.SetValue(1 << 1);
                or2.SetValue(1 << 1);
                Thread.Sleep(100);
                or1.SetValue(1 << 2);
                or2.SetValue(1 << 2);
                Thread.Sleep(100);
                or1.SetValue(1 << 3);
                or2.SetValue(1 << 3);
            }

            or1.SetValue(0);
            or2.SetValue(0);

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
            ipcon.Disconnect();
        }
    }
}
