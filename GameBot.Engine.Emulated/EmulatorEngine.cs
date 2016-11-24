using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Emulation;
using System;
using GameBot.Core.Engines;

namespace GameBot.Engine.Emulated
{
    public class EmulatorEngine : BaseEngine
    {
        private readonly IConfig _config;
        private readonly Emulator _emulator;

        public EmulatorEngine(IConfig config, ICamera camera, IClock clock, IExecutor executor, IQuantizer quantizer, IAgent agent, Emulator emulator) : base(camera, clock, executor, quantizer, agent)
        {
            _config = config;

            _emulator = emulator;
            LoadRom();
        }

        private void LoadRom()
        {
            var romPath = _config.Read("Emulator.Rom.Path", "Roms/tetris.gb");
            var game = new RomLoader().Load(romPath);

            lock (_emulator)
            {
                _emulator.Load(game);
            }
        }

        public override void Step(Action<Mat> showImage = null, Action<Mat> showProcessedImage = null)
        {
            // get image as photo of the gameboy screen (input)
            Mat image = Camera.Capture();
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

            lock (_emulator)
            {
                _emulator.Execute();
            }
        }
    }
}
