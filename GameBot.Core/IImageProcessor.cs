using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Core.ImageProcessing
{
    public interface IImageProcessor
    {
        IDisplayState Process(Image image);
    }
}
