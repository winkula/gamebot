using Emgu.CV;
using Emgu.CV.Structure;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Ui;
using System;
using System.Collections.Generic;

namespace GameBot.Robot.Engines
{
    public class UiEngine : IEngine
    {
        private readonly IConfig config;

        private readonly ICamera camera;
        private readonly IQuantizer quantizer;
        private readonly IAgent agent;
        private readonly IExecutor executor;
        private readonly ITimeProvider timeProvider;
        private readonly IDebugger debugger;

        public UiEngine(IConfig config, ICamera camera, IQuantizer quantizer, IAgent agent, IExecutor executor, ITimeProvider timeProvider, IDebugger debugger)
        {
            this.config = config;

            this.camera = camera;
            this.quantizer = quantizer;
            this.agent = agent;
            this.executor = executor;
            this.timeProvider = timeProvider;
            this.debugger = debugger;
        }

        public void Run()
        {
            throw new NotSupportedException("Can only be called step by step.");
        }

        public EngineResult Initialize()
        {
            timeProvider.Start();

            return RunInternal(true);
        }

        public EngineResult Step()
        {
            return RunInternal(false);
        }

        private EngineResult RunInternal(bool initialize)
        {
            var result = new EngineResult();
            
            // get image as photo of the gameboy screen (input)
            IImage image = camera.Capture();
            result.Original = image;

            // process image and get display data
            TimeSpan time = timeProvider.Time;
            IImage processed = quantizer.Quantize(image);
            IScreenshot screenshot = new EmguScreenshot(processed, time);

            processed = agent.Visualize(processed);
            result.Processed = new Image<Bgr, byte>(processed.Bitmap);

            // handle input to the agent which
            //  - extracts the game state
            //  - decides which commands to press
            IEnumerable<ICommand> commands;
            
            if (initialize)
                commands = agent.Initialize();
            else
                commands = agent.Act(screenshot);

            // give commands to command controller (output)
            executor.Execute(commands);

            debugger.WriteStatic(DateTime.Now);

            return result;
        }
    }
}
