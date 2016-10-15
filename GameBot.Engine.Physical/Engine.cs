using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using System;

namespace GameBot.Engine.Physical
{
    public class Engine : IEngine
    {
        private readonly IConfig _config;

        private readonly IActuator _actuator;
        private readonly ICamera _camera;
        private readonly IClock _clock;
        private readonly IExecutor _executor;
        private readonly IQuantizer _quantizer;
        private readonly IAgent _agent;

        public bool Play { get; set; }

        public Engine(IConfig config, ICamera camera, IExecutor executor, IQuantizer quantizer, IAgent agent, IActuator actuator, IClock clock)
        {
            _config = config;

            _camera = camera;
            _quantizer = quantizer;
            _agent = agent;
            _actuator = actuator;
            _clock = clock;
        }

        public void Initialize()
        {
            _clock.Start();
        }

        public void Step(Action<IImage, IImage> callback = null)
        {
            // get image as photo of the gameboy screen (input)
            IImage image = _camera.Capture();

            // process image and get display data
            TimeSpan time = _clock.Time;
            IImage processed = _quantizer.Quantize(image);

            callback(image, processed);

            if (Play)
            {
                IScreenshot screenshot = new EmguScreenshot(processed, time);

                // handle input to the agent which
                //  - extracts the game state
                //  - decides which commands to press
                //  - presses the buttons
                _agent.Act(screenshot, _executor);
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
