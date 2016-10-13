using GameBot.Core.Configuration;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris
{
    [TestFixture]
    public class TetrisAiTests
    {
        [Test]
        public void Constructor()
        {
            var player = new TetrisAi(new AppSettingsConfig());
        }

        [Test]
        public void Play()
        {
            var player = new TetrisAi(new AppSettingsConfig());
            var gameState = new GameState(Tetromino.L, Tetromino.Z);
            
            var commands = player.Play(gameState);
            
            Assert.NotNull(commands);
        }
    }
}
