namespace GameBot.Game.Tetris
{
    /// <summary>
    /// On level 0, the player gets 40 points for a Single, 100 points for a Double,
    /// 300 points for a Triple and 1200 points for a Tetris. On higher levels, those
    /// points are multiplied by (level + 1). On the other hand, softdrop points are
    /// not multiplied by the current level.
    /// The "heart levels" (activated by holding Down+Start at the title screen) are
    /// as fast as the level plus 10, but unlike on the NES version, they don't improve
    /// the score.
    /// The maximum Score is 999,999.
    /// http://harddrop.com/wiki/Tetris_(Game_Boy)
    /// </summary>
    public class TetrisScore
    {
        private const int _scoreSoftDrop = 1;
        private const int _scoreSingle = 40;
        private const int _scoreDouble = 100;
        private const int _scoreTriple = 300;
        private const int _scoreTeris = 1200;
        private static readonly int[] _linesScores = { 0, _scoreSingle, _scoreDouble, _scoreTriple, _scoreTeris };
        
        public static int GetSoftdropScore(int fallHeight)
        {
            return fallHeight * _scoreSoftDrop;
        }

        public static int GetLineScore(int clearedLines, int level)
        {
            return GetMultiplier(level) * _linesScores[clearedLines];
        }

        private static int GetMultiplier(int level)
        {
            return 1 + level;
        }
    }
}
