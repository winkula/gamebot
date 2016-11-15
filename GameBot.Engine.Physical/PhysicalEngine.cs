using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using System;

namespace GameBot.Engine.Physical
{
    public class PhysicalEngine : IEngine
    {
        private readonly ICamera _camera;
        private readonly IClock _clock;
        private readonly IExecutor _executor;
        private readonly IQuantizer _quantizer;

        private readonly IAgent _agent;

        public bool Play { get; set; }

        public PhysicalEngine(ICamera camera, IExecutor executor, IQuantizer quantizer, IAgent agent, IClock clock)
        {
            _camera = camera;
            _clock = clock;
            _executor = executor;
            _quantizer = quantizer;

            _agent = agent;
        }

        public void Initialize()
        {
            _clock.Start();
        }

        public void Step(Action<Mat> showImage = null, Action<Mat> showProcessedImage = null)
        {
            // get image as photo of the gameboy screen (input)
            Mat image = _camera.Capture();
            TimeSpan time = _clock.Time;

            // process image
            Mat processed = _quantizer.Quantize(image);

            showImage?.Invoke(image);

            if (Play)
            {
                IScreenshot screenshot = new EmguScreenshot(processed, time);
                screenshot.OriginalImage = image;

                // extracts the game state
                _agent.Extract(screenshot);

                processed = _agent.Visualize(processed);
                showProcessedImage?.Invoke(processed);

                // presses the buttons
                _agent.Play(_executor);
            }
            else
            {
                showProcessedImage?.Invoke(processed);
            }
        }

        public void Reset()
        {
            Play = false;
            _agent.Reset();
        }
    }
}
