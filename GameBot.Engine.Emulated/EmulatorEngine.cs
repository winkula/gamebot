using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Emulation;
using NLog;
using System;

namespace GameBot.Engine.Emulated
{
    public class EmulatorEngine : IEngine
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IConfig config;

        private readonly IActuator actuator;
        private readonly ICamera camera;
        private readonly IClock clock;
        private readonly IQuantizer quantizer;

        private readonly IAgent agent;

        private readonly Emulator emulator;

        public bool Play { get; set; }

        public EmulatorEngine(IConfig config, IActuator actuator, ICamera camera, IClock clock, IQuantizer quantizer, IAgent agent, Emulator emulator)
        {
            this.config = config;

            this.actuator = actuator;
            this.camera = camera;
            this.clock = clock;
            this.quantizer = quantizer;

            this.agent = agent;

            this.emulator = emulator;
            LoadRom();
        }

        private void LoadRom()
        {
            var loader = new RomLoader();
            var game = loader.Load(config.Read("Emulator.Rom.Path", "Roms/tetris.gb"));
            emulator.Load(game);
        }

        public void Initialize()
        {
            clock.Start();
        }

        public void Step(Action<IImage,IImage> callback = null)
        {
            // get image as photo of the gameboy screen (input)
            IImage image = camera.Capture();

            // process image and get display data
            TimeSpan time = clock.Time;
            IImage processed = quantizer.Quantize(image);
            
            if (Play)
            {
                IScreenshot screenshot = new EmguScreenshot(processed, time);

                // handle input to the agent which
                //  - extracts the game state
                //  - decides which commands to press
                //  - presses the commands
                agent.Act(screenshot, actuator);
                processed = agent.Visualize(processed);
            }

            callback?.Invoke(image, processed);

            emulator.Execute();
        }
    }
}
