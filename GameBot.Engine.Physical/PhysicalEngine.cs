using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using System;
using GameBot.Core.Engines;

namespace GameBot.Engine.Physical
{
    public class PhysicalEngine : BaseEngine
    {
        private Mat _lastImage;

        public PhysicalEngine(ICamera camera, IClock clock, IExecutor executor, IQuantizer quantizer, IAgent agent) : base(camera, clock, executor, quantizer, agent)
        {
        }

        public override void Step(Action<Mat> showImage = null, Action<Mat> showProcessedImage = null)
        {
            // get image as photo of the gameboy screen (input)
            Mat image = Camera.Capture(_lastImage);
            _lastImage = image;
            TimeSpan time = Clock.Time;

            // process image
            Mat processed = Quantizer.Quantize(image);

            showImage?.Invoke(image);

            if (Play)
            {
                IScreenshot screenshot = new EmguScreenshot(processed, time);
                screenshot.OriginalImage = image;

                // extracts the game state
                Agent.Extract(screenshot);

                processed = Agent.Visualize(processed);
                showProcessedImage?.Invoke(processed);

                // presses the buttons
                Agent.Play(Executor);
            }
            else
            {
                showProcessedImage?.Invoke(processed);
            }
        }
    }
}
