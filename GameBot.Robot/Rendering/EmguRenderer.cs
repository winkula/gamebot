using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace GameBot.Robot.Rendering
{
    public class EmguRenderer : IRenderer
    {
        private string title;
        private Mat img;

        public void OpenWindow(string title)
        {
            this.title = title;
            CvInvoke.NamedWindow(title);
        }

        public void CreateImage(int width, int height)
        {
            img = new Mat(width, height, DepthType.Cv8U, 3);
            img.SetTo(new Bgr(0, 0, 0).MCvScalar);
        }

        public void Render(Image image)
        {
            Image<Bgr, Byte> toRender = new Image<Bgr, Byte>(new Bitmap(image));

            img.SetTo(new Bgr(255, 77, 0).MCvScalar);
            /*

            CvInvoke.PutText(
               img,
               string.Format("bla"),
               new Point(10, 80),
               FontFace.HersheySimplex,
               1.0,
               new Bgr(0, 0, 0).MCvScalar);
           */
            CvInvoke.Imshow(title, toRender);
        }

        public int? Key(int delay = 0)
        {
            // Wait for the key pressing event
            int key = CvInvoke.WaitKey(delay);
            if (key == -1) return null;
            return key;
        }

        public void End()
        {
            // Destroy the window if key is pressed
            CvInvoke.DestroyWindow(title);
        }
    }
}
