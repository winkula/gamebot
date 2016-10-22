using System;
using System.IO;
using GameBot.Core.Data;

namespace GameBot.Core.Extensions
{
    public static class ScreenshotExtensions
    {
        public static void Save(this IScreenshot screenshot, string message)
        {
            string outputFilename = $"{DateTime.Now:HH_mm_ss_ffff}_{message}.png";
            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "debug", outputFilename);
            screenshot.Image.Save(outputPath);
        }
    }
}
