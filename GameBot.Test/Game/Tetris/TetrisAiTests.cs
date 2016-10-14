using GameBot.Core;
using GameBot.Core.Configuration;
using GameBot.Game.Tetris;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Searching.Heuristics;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris
{
    [TestFixture]
    public class TetrisAiTests
    {
        private IConfig config;
        private ISearch search;

        [SetUp]
        public void Init()
        {
            config = new AppSettingsConfig();
            search = new SimpleSearch(new YiyuanLeeHeuristic());
        }

        [Test]
        public void Constructor()
        {
            var player = new TetrisAi(config, search);
        }

        [Test]
        public void Play()
        {
            var player = new TetrisAi(config, search);
            var gameState = new GameState(Tetromino.L, Tetromino.Z);
            
            var commands = player.Play(gameState);
            
            Assert.NotNull(commands);
        }
    }
}
