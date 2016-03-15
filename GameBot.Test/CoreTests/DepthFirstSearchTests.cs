using GameBot.Core.Searching;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System.Diagnostics;

namespace GameBot.Test
{
    [TestFixture]
    public class DepthFirstSearchTest
    {
        [Test]
        public void Constructor()
        {
            int bound = 2;

            var search = new DepthFirstSearch(bound);

            Assert.AreEqual(bound, search.Bound);
        }

        [Test]
        public void Search()
        {
            var search = new DepthFirstSearch();            
            var gameState = new TetrisGameState(Tetromino.L, Tetromino.Z);

            var node = new TetrisNode(gameState);
            
            var winner = search.Search(node);
            Debug.WriteLine(winner);
        }
    }
}
