using GameBot.Game.Tetris.Data;
using NUnit.Framework;

namespace GameBot.Test.Tetris
{
    [TestFixture]
    public class PieceExtractorTests
    {
        [Test]
        public void BuildDatabasePieceMasks()
        {
            // TODO: implement

            // analyze every possibility of masks
            ushort mask = 0;
            for (int i = 0; i < 65536; i++)
            {
                foreach (var pose in Pose.All)
                {

                }
                mask++;
            }
        }
    }
}
