using Emgu.CV;
using Emgu.CV.Structure;

namespace GameBot.Core.Data
{
    public class EngineResult
    {
        public IImage Original { get; set; }
        public Image<Bgr, byte> Processed { get; set; }
    }
}
