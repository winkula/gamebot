namespace GameBot.Game.Tetris
{
    // TODO: finetune!
    public class Timing
    {
        private const int _fps = 20;

        private const int _roundtripTime = 1000 / _fps; // ms
        
        // we can miss 3 frames
        public const int ExpectedFallDurationPadding = 3 * _roundtripTime; // three cycles at 20 fps (ms)

        public const int NegativeDropDurationPadding = 3 * _roundtripTime; // the cycle at 20 fps (ms)
        
        public const int TimeAfterButtonPress = 200; // ms

        public const int PaddingAnalyze = 200; // ms
    }
}
