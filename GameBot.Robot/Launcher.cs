using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Threading;

namespace GameBot.Robot
{
    public class Launcher
    {
        static void Main(string[] args)
        {
            String win1 = "Test Window"; //The name of the window
            CvInvoke.NamedWindow(win1); //Create the window using the specific name

            Mat img = new Mat(200, 400, DepthType.Cv8U, 3); //Create a 3 channel image of 400x200
            img.SetTo(new Bgr(255, 77, 0).MCvScalar); // set it to Blue color

            int key = 0;
            for (int i = 0; i < 10 * 60; i++)
            {
                img.SetTo(new Bgr(255, 77, 0).MCvScalar); // set it to Blue color
                //Draw "Hello, world." on the image using the specific font
                CvInvoke.PutText(
                   img,
                   string.Format("Frame = {0}. Key = {1}", i, key),
                   new System.Drawing.Point(10, 80),
                   FontFace.HersheySimplex,
                   1.0,
                   new Bgr(key % 255, key % (6*255), key % (2)).MCvScalar);


                CvInvoke.Imshow(win1, img); //Show the image
                //Thread.Sleep(300);
                var pressedKey = CvInvoke.WaitKey(1);  //Wait for the key pressing event
                if (pressedKey != -1)
                {
                    key = pressedKey;
                }
            }
            CvInvoke.WaitKey(0);  //Wait for the key pressing event
            CvInvoke.DestroyWindow(win1); //Destroy the window if key is pressed

            // async emulator example:
            /*
            var emulator = new GameBoyEmulatorAsync();
            
            emulator.Init(@"C:\Users\Winkler\Desktop\gb_output.bmp");
            emulator.Open(@"C:\Users\Winkler\Documents\Visual Studio 2015\Projects\GameBot\Roms\tetris.gb");
            emulator.Run(() => { });*/
        }
    }
}
