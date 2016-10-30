using System;
using System.IO;
using Emgu.CV;

namespace GameBot.Core.Extensions
{
    public static class ImageExtensions
    {
        public static void SaveToDesktop(this IImage image, string name)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{name}.png");
            image.Save(path);
        }
    }
}
