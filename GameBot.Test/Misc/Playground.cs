using Emgu.CV;
using Emgu.CV.CvEnum;
using NUnit.Framework;
using System;
using System.IO;

namespace GameBot.Test.Misc
{
    [Ignore]
    [TestFixture]
    public class Playground
    {
        [Test]
        public void CreateBinaryTemplates()
        {
            // source image
            string path = @"C:\Users\Winkler\Desktop\TemplatesGrayscale.png";
            var image = new Mat(path, LoadImageType.Grayscale);

            CvInvoke.AdaptiveThreshold(image, image, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 3, 5);

            // open window
            CvInvoke.Imshow("test", image);
            
            // show/save 
            string outputFilename = "TemplatesBinary.png";
            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), outputFilename);
            image.Save(outputPath);
        }
    }
}
