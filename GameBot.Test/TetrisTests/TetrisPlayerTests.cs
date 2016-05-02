using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Heuristics;
using GameBot.Robot.Configuration;
using NUnit.Framework;

namespace GameBot.Test.TetrisTests
{
    [TestFixture]
    public class TetrisPlayerTests
    {
        [Test]
        public void Constructor()
        {
            var player = new TetrisPlayer(new Config());
        }

        [Test]
        public void Play()
        {
            var player = new TetrisPlayer(new Config());
            var gameState = new TetrisGameState(Tetromino.L, Tetromino.Z);

            var commandsBegin = player.Play(gameState);
            var commands = player.Play(gameState);

            Assert.NotNull(commandsBegin);
            Assert.NotNull(commands);
        }
    }
}
