using System;
using Tinkerforge;

namespace GameBot.Robot
{
    public class ExamplesTinkerforge
    {
        private static string HOST = "localhost";
        private static int PORT = 4223;
        private static string UID = "XYZ"; // Change to your UID

        public static void Input()
        {
            IPConnection ipcon = new IPConnection(); // Create IP connection
            BrickletIO16 io = new BrickletIO16(UID, ipcon); // Create device object

            ipcon.Connect(HOST, PORT); // Connect to brickd
                                       // Don't use device before ipcon is connected

            // Get current value from port A as bitmask
            byte valueMask = io.GetPort('a');
            Console.WriteLine("Value Mask (Port A): " + Convert.ToString(valueMask, 2));

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
            ipcon.Disconnect();
        }

        public static void Output()
        {
            IPConnection ipcon = new IPConnection(); // Create IP connection
            BrickletIO16 io = new BrickletIO16(UID, ipcon); // Create device object

            ipcon.Connect(HOST, PORT); // Connect to brickd
                                       // Don't use device before ipcon is connected

            // Set pin 0 on port A to output low
            io.SetPortConfiguration('a', 1 << 0, 'o', false);

            // Set pin 0 and 7 on port B to output high
            io.SetPortConfiguration('b', (1 << 0) | (1 << 7), 'o', true);

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
            ipcon.Disconnect();
        }

        public static void Interrupt()
        {
            IPConnection ipcon = new IPConnection(); // Create IP connection
            BrickletIO16 io = new BrickletIO16(UID, ipcon); // Create device object

            ipcon.Connect(HOST, PORT); // Connect to brickd
                                       // Don't use device before ipcon is connected

            // Register interrupt callback to function InterruptCB
            io.Interrupt += InterruptCB;

            // Enable interrupt on pin 2 of port A
            io.SetPortInterrupt('a', 1 << 2);

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
            ipcon.Disconnect();
        }

        // Callback function for interrupt callback
        private static void InterruptCB(BrickletIO16 sender, char port, byte interruptMask, byte valueMask)
        {
            Console.WriteLine("Port: " + port);
            Console.WriteLine("Interrupt Mask: " + Convert.ToString(interruptMask, 2));
            Console.WriteLine("Value Mask: " + Convert.ToString(valueMask, 2));
            Console.WriteLine("");
        }
    }
}
