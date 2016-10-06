using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using NUnit.Framework;
using System.Diagnostics;

namespace GameBot.Test.Tetris.Extraction
{
    [TestFixture]
    public class PieceMatcherTests
    {
        private PieceMatcher matcher;

        [TestFixtureSetUp]
        public void Init()
        {
            matcher = PieceMatcher.Instance;
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
