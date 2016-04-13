using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Heuristics;
using NUnit.Framework;

namespace GameBot.Test.TetrisTests
{
    [TestFixture]
    public class TetrisSolverTests
    {
        [Test]
        public void Constructor()
        {
            var solver = new TetrisSolver(new TetrisSearch(new TetrisSurviveHeuristic()));
        }

        [Test]
        public void Solve()
        {
            var solver = new TetrisSolver(new TetrisSearch(new TetrisSurviveHeuristic()));
            var gameState = new TetrisGameState(Tetromino.L, Tetromino.Z);

            var decision = solver.Solve(gameState);

            Assert.NotNull(decision);
        }
    }
}
