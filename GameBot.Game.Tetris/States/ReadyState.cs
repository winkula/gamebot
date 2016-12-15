using NLog;

namespace GameBot.Game.Tetris.States
{
    public class ReadyState : BaseState
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ReadyState(TetrisAgent agent) : base(agent)
        {
            _logger.Info("Agent is ready");
        }

        public override void Extract()
        {
            if (Agent.ScreenExtractor.IsStart(Screenshot))
            {
                SetStateAndContinue(new AnalyzeState(Agent, Screenshot.Timestamp));
            }
        }
    }
}
