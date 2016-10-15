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
        private IConfig _config;
        private ISearch _search;

        [SetUp]
        public void Init()
        {
            _config = new AppSettingsConfig();
            _search = new SimpleSearch(new YiyuanLeeHeuristic());
        }

        [Test]
        public void Constructor()
        {
            var player = new TetrisAi(_config, _search);
        }

        [Test]
        public void Play()
        {
            var player = new TetrisAi(_config, _search);
            var gameState = new GameState(Tetromino.L, Tetromino.Z);
            
            var commands = player.Play(gameState);
            
            Assert.NotNull(commands);
        }
    }
}
