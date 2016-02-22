using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Core
{
    public interface IDisplayState
    {
        byte[] GetPixels();

        byte GetPixel(int x, int y);

        TimeSpan GetTimestamp();
    }
}
