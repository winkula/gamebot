using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Emulation;
using NLog;
using System;
using System.Diagnostics;

namespace GameBot.Engine.Emulated
{
    public class EmulatorEngine : IEngine
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IConfig _config;
        
        private readonly ICamera _camera;
        private readonly IClock _clock;
        private readonly IExecutor _executor;
        private readonly IQuantizer _quantizer;

        private readonly IAgent _agent;

        private readonly Emulator _emulator;

        public bool Play { get; set; }

        public EmulatorEngine(IConfig config, ICamera camera, IClock clock, IExecutor executor, IQuantizer quantizer, IAgent agent, Emulator emulator)
        {
            _config = config;
            
            _camera = camera;
            _clock = clock;
            _executor = executor;
            _quantizer = quantizer;

            _agent = agent;

            _emulator = emulator;
            LoadRom();
        }

        private void LoadRom()
        {
            var loader = new RomLoader();
            var game = loader.Load(_config.Read("Emulator.Rom.Path", "Roms/tetris.gb"));
            lock (_emulator)
            {
                _emulator.Load(game);
            }
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

            lock (_emulator)
            {
                _emulator.Execute();
            }
        }

        public void Reset()
        {
            Play = false;
            _agent.Reset();
        }
    }
}
