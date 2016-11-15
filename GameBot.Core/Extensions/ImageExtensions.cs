using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Emgu.CV;

namespace GameBot.Core.Extensions
{
    public static class ImageExtensions
    {
        public static void SaveToDesktop(this Mat image, string name)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{name}.png");
            image.Save(path);
        }

        public static byte[] ToByteArray(this Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }
    }
}
