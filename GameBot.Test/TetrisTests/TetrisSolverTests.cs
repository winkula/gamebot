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

namespace GameBot.Test.TetrisTests
{
    [TestFixture]
    public class TetrisSolverTests
    {
        [Test]
        public void Constructor()
        {
            var solver = new TetrisSolver();
        }

        [Test]
        public void Solve()
        {
            var solver = new TetrisSolver();
            var gameState = new TetrisGameState(Tetromino.L, Tetromino.Z);

            var decision = solver.Solve(gameState);

            Assert.NotNull(decision);
        }
    }
}
