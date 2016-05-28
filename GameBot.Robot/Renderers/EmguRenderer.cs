using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace GameBot.Robot.Renderers
{
    /*
    public class EmguRenderer : IRenderer
    {
        private string title;
        private Mat mat;

        public void OpenWindow(string title)
        {
            this.title = title;
            CvInvoke.NamedWindow(title);
        }

        public void CreateImage(int width, int height)
        {
            mat = new Mat(width, height, DepthType.Cv8U, 3);
            mat.SetTo(new Bgr(0, 0, 0).MCvScalar);
        }

        public void Render(Image image, string title)
        {
            Image<Bgr, Byte> toRender = new Image<Bgr, Byte>(new Bitmap(image));           
            CvInvoke.Imshow(title, toRender);
        }

        public void Render(IImage image, string title)
        {
            CvInvoke.Imshow(title, image);
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
    }*/
}
