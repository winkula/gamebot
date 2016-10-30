using System;
using System.IO;
using System.Linq;
using GameBot.Core.Data;

namespace GameBot.Core.Extensions
{
    public static class ScreenshotExtensions
    {
        public static void Save(this IScreenshot screenshot, IQuantizer quantizer, string message)
        {
            string pathDebug = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                "debug",
                $"{DateTime.Now:HH_mm_ss_ffff}_{message}.png");
            screenshot.Image.Save(pathDebug);
            
            string keypoints = string.Join("_", quantizer.Keypoints.Select(p => $"{p.X}_{p.Y}"));
            string filename = $"{DateTime.Now:HH_mm_ss_ffff}_{message}_{keypoints}.png";
            string pathTest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                "test",
                filename);
            screenshot.OriginalImage.Save(pathTest);
        }
    }
}
