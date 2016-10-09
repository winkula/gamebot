namespace GameBot.Game.Tetris
{
    public class Timing
    {
        public const int Fps = 20;

        public const int RoundtripTime = 1000 / Fps; // ms

        // we can miss 3 frames
        public const int ExpectedFallDurationPadding = 3 * RoundtripTime; // three cycles at 20 fps

        public const int NegativeDropDurationPadding = 1 * RoundtripTime; // one cycle at 20 fps
    }
}
