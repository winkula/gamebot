namespace GameBot.Game.Tetris.Data
{
    public enum Tetromino
    {
        O = 0,
        I = 1,
        S = 2,
        Z = 3,
        L = 4,
        J = 5,
        T = 6
    }

    public static class TetrominoExtensions
    {
        public static double GetChance(this Tetromino tetromino)
        {
            // TODO: take real chances!
            return 1.0 / 7.0;
        }
    }
}
