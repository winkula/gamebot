using Emgu.CV;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace GameBot.Test
{
    [TestFixture]
    public class DeciderTests
    {
        [Test]
        public void Constructor()
        {
            var decider = new TetrisDecider();
        }

        [Test]
        public void Decide()
        {
            var decider = new TetrisDecider();
            var gameState = new TetrisGameStateFull();
            gameState.State = new TetrisGameState(Tetromino.L, Tetromino.Z);

            var decision = decider.Decide(gameState, new Context<TetrisGameStateFull>());

            Assert.NotNull(decision);
        }
    }
}
