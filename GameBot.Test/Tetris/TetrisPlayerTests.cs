using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using GameBot.Robot.Configuration;
using NUnit.Framework;

namespace GameBot.Test.Tetris
{
    [TestFixture]
    public class TetrisPlayerTests
    {
        [Test]
        public void Constructor()
        {
            var player = new TetrisAi(new Config());
        }

        [Test]
        public void Play()
        {
            var player = new TetrisAi(new Config());
            var gameState = new TetrisGameState(Tetromino.L, Tetromino.Z);
            
            var commands = player.Play(gameState);
            
            Assert.NotNull(commands);
        }
    }
}
