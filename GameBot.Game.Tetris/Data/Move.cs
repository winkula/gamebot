namespace GameBot.Game.Tetris.Data
{
    public class Move
    {
        public int Rotation { get; }
        public int Translation { get; }
        public int Fall { get; }

        public Move(int rotation, int translation, int fall)
        {
            Rotation = rotation;
            Translation = translation;
            Fall = fall;
        }

        public override string ToString()
        {
            return string.Format("Move {{ Rotation: {0}, Translation: {1}, Fall: {2} }}", Rotation, Translation, Fall);
        }
    }
}
