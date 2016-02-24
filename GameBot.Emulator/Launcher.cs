using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Timers;

namespace GameBot.Emulator
{
    public class Launcher
    {
        [STAThread]
        static void Main(string[] args)
        {
            /*
            var emulator = new GameBoyEmulatorAsync();
            
            emulator.Init(@"C:\Users\Winkler\Desktop\gb_output.bmp");
            emulator.Open(@"C:\Users\Winkler\Documents\Visual Studio 2015\Projects\GameBot\Roms\tetris.gb");
            emulator.Run(() =>
            {
                if (DateTime.Now.Millisecond % 100 == 0)
                {
                    emulator.KeyDown(Keys.Enter);
                }
                if (DateTime.Now.Millisecond % 100 == 50)
                {
                    emulator.KeyUp(Keys.Enter);
                }
                if (DateTime.Now.Millisecond % 100 == 99)
                {
                    Console.WriteLine(emulator.GetCatridgeInfo());
                }
            });*/
        }
    }
}
