using System;
using System.Text;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace GameBot.Core.Extensions
{
    public static class CaptureExtensions
    {
        public static string ReadProperties(this Capture capture)
        {
            var sb = new StringBuilder();

            foreach (var key in Enum.GetValues(typeof(CapProp)))
            {
                var value = capture.GetCaptureProperty((CapProp)key);
                if (value != -1)
                {
                    sb.AppendLine($"{key,25}: {value}");
                }
            }

            return sb.ToString();
        }
    }
}
