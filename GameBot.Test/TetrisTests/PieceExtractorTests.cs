using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Extraction;
using NUnit.Framework;
using System.Diagnostics;

namespace GameBot.Test.TetrisTests
{
    [TestFixture]
    public class PieceExtractorTests
    {
        [Test]
        public void Constructor()
        {
            var pieceExtractor = new PieceExtractor();
        }
    }
}
