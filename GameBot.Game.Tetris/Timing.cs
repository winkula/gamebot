namespace GameBot.Game.Tetris
{
    public class Timing
    {
        private const int _fps = 20;

        private const int _roundtripTime = 1000 / _fps; // ms
        
        // we can miss 3 frames
        public const int ExpectedFallDurationPadding = 3 * _roundtripTime; // three cycles at 20 fps

        public const int NegativeDropDurationPadding = 1 * _roundtripTime; // one cycle at 20 fps
    }
}
