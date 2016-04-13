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
        public static int ScoreSoftDrop = 1;
        public static int ScoreSingle = 40;
        public static int ScoreDouble = 100;
        public static int ScoreTriple = 300;
        public static int ScoreTeris = 1200;
        public static int[] LinesScores = new int[] { 0, ScoreSingle, ScoreDouble, ScoreTriple, ScoreTeris };

        public static int GetSoftdropScore(int fallHeight)
        {
            return fallHeight * ScoreSoftDrop;
        }

        public static int GetLineScore(int clearedLines, int level)
        {
            return GetMultiplier(level) * LinesScores[clearedLines];
        }

        private static int GetMultiplier(int level)
        {
            return 1 + level;
        }
    }
}
