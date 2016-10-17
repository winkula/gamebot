namespace GameBot.Game.Tetris.Data
{
    // TODO: remove this class and use Piece or PieceDelta?
    public class Way
    {
        public int Rotation { get; }
        public int Translation { get; }
        public int Fall { get; }

        public Way(int rotation, int translation, int fall = 0)
        {
            Rotation = rotation;
            Translation = translation;
            Fall = fall;
        }

        public override string ToString()
        {
            return $"Move {{ Rotation: {Rotation}, Translation: {Translation}, Fall: {Fall} }}";
        }
    }
}
