using GameBot.Game.Tetris.Extraction;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris.Extraction
{
    [TestFixture]
    public class PieceMatcherTests
    {
        private PieceMatcher _matcher;

        [TestFixtureSetUp]
        public void Init()
        {
            _matcher = PieceMatcher.Instance;
        }

        [TestCase(new[] {
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0
        })]
        public void Test(int[] input)
        {
        }
    }
}
