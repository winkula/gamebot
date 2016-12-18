using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV;
using GameBot.Core.Data;
using GameBot.Core.Exceptions;

namespace GameBot.Core.Engines
{
    public abstract class BaseEngine : IEngine
    {
        protected readonly ICamera Camera;
        protected readonly IClock Clock;
        protected readonly IExecutor Executor;
        protected readonly IQuantizer Quantizer;
        protected readonly IAgent Agent;

        public bool Play { get; set; }

        private readonly Queue<string> _messages = new Queue<string>();

        protected BaseEngine(ICamera camera, IClock clock, IExecutor executor, IQuantizer quantizer, IAgent agent)
        {
            Camera = camera;
            Clock = clock;
            Executor = executor;
            Quantizer = quantizer;

            Agent = agent;
        }

        public void Initialize()
        {
            Clock.Start();
        }

        public void Step(Action<Mat> showImage = null, Action<Mat> showProcessedImage = null)
        {
            // get image as photo of the gameboy screen (input)
            Mat image = Capture();
            TimeSpan time = Clock.Time;

            // process image
            Mat processed = Quantizer.Quantize(image);

            showImage?.Invoke(image);

            if (_messages.Count > 0)
            {
                // send messages to agent
                Agent.Send(PopAllMessages());
            }

            if (Play)
            {
                IScreenshot screenshot = new EmguScreenshot(processed, time);
                screenshot.OriginalImage = image;

                try
                {
                    // extracts the game state
                    Agent.Extract(screenshot);

                    processed = Agent.Visualize(processed);
                    showProcessedImage?.Invoke(processed);

                    // presses the buttons
                    Agent.Play(Executor);
                }
                catch (GameOverException)
                {
                    OnGameOver();
                }
            }
            else
            {
                showProcessedImage?.Invoke(processed);
            }

            OnAfterStep();
        }

        protected virtual Mat Capture()
        {
            return Camera.Capture();
        }

        protected virtual void OnAfterStep()
        {
        }

        protected virtual void OnGameOver()
        {
            Reset();
        }

        public void Reset()
        {
            Play = false;
            Agent.Send(new[] { "reset" });
        }

        public void Send(string message)
        {
            PushMessage(message);
        }

        private void PushMessage(string message)
        {
            lock (_messages)
            {
                _messages.Enqueue(message);
            }
        }

        private ICollection<string> PopAllMessages()
        {
            IList<string> all;

            lock (_messages)
            {
                all = _messages.ToList();
                _messages.Clear();
            }

            return all;
        }
    }
}
