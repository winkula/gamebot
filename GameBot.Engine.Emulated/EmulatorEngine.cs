using GameBot.Core;
using GameBot.Emulation;
using GameBot.Core.Engines;
using NLog;

namespace GameBot.Engine.Emulated
{
    public class EmulatorEngine : BaseEngine
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

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

        protected override void OnGameOver()
        {
            _logger.Warn("Game over");

            base.OnGameOver();
        }

        protected override void OnAfterStep()
        {
            lock (_emulator)
            {
                _emulator.Execute(2);
            }
        }
    }
}
