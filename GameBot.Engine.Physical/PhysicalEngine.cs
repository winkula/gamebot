using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Engines;
using NLog;

namespace GameBot.Engine.Physical
{
    public class PhysicalEngine : BaseEngine
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private Mat _lastImage;

        public PhysicalEngine(ICamera camera, IClock clock, IExecutor executor, IQuantizer quantizer, IAgent agent) : base(camera, clock, executor, quantizer, agent)
        {
        }

        protected override Mat Capture()
        {
            Mat image = Camera.Capture(_lastImage);
            _lastImage = image;

            return image;
        }

        protected override void OnGameOver()
        {
            _logger.Warn("Game over");

            base.OnGameOver();
        }
    }
}
