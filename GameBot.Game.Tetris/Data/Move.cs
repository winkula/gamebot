namespace GameBot.Game.Tetris.Data
{
    public enum Move
    {
        // TODO: do we need this?
        None = 0,
        
        Left = 1,
        Right = 2,

        Rotate = 3,
        RotateCounterclockwise = 4,

        Fall = 5,
        Drop = 6
    }
}
