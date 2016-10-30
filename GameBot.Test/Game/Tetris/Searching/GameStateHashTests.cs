using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching.Heuristics;
using NLog;
using NUnit.Framework;
using System.Linq;

namespace GameBot.Test.Game.Tetris.Searching
{
    [Ignore]
    [TestFixture]
    public class GameStateHashTests
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const int _numberOfGameState = 100;

        private IHeuristic _heuristic;
        
        [TestFixtureSetUp]
        public void Init()
        {
            _heuristic = new YiyuanLeeHeuristic();
        }

        [Test]
        public void RandomlyGenerateBoards()
        {
            int number = 10;

            var series1 = GenerateGameStates(number).ToList();
            var series2 = GenerateGameStates(number).ToList();

            Assert.NotNull(series1);
            Assert.NotNull(series2);
            Assert.AreEqual(number, series1.Count);
            Assert.AreEqual(number, series2.Count);

            for (int i = 0; i < number; i++)
            {
                Assert.AreEqual(series1[i], series2[i]);
                Assert.AreEqual(series1[i].GetHashCode(), series2[i].GetHashCode());

                //_logger.Info(series1[i]);
            }
        }

        [Test]
        public void CalculateScores()
        {
            var gameStates = GenerateGameStates(_numberOfGameState).ToList();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var gameState in gameStates)
            {
                var score = _heuristic.Score(gameState);
            }

            stopwatch.Stop();
            _logger.Info($"Time for CalculateScores: {stopwatch.ElapsedMilliseconds} ms");
            _logger.Info($"{gameStates.Count} game states");
        }

        [Test]
        public void CalculateScoresAndHash()
        {
            var gameStates = GenerateGameStates(_numberOfGameState).ToList();
            var dictionary = new Dictionary<GameState, double>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var gameState in gameStates)
            {
                if (!dictionary.ContainsKey(gameState))
                {
                    var score = _heuristic.Score(gameState);
                    dictionary.Add(gameState, score);
                }
            }

            stopwatch.Stop();
            _logger.Info($"Time for CalculateScoresAndHash: {stopwatch.ElapsedMilliseconds} ms");
            _logger.Info($"{gameStates.Count} game states");
            _logger.Info($"Dictionary contains {dictionary.Count} entries");
        }

        [Test]
        public void GetByHashes()
        {
            var gameStates = GenerateGameStates(_numberOfGameState).ToList();
            var dictionary = BuildDictionary(gameStates);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var gameState in gameStates)
            {
                var score = dictionary[gameState];
            }
            
            stopwatch.Stop();
            _logger.Info($"Time for GetByHashes: {stopwatch.ElapsedMilliseconds} ms");
            _logger.Info($"{gameStates.Count} game states");
            _logger.Info($"Dictionary contains {dictionary.Count} entries");
        }

        private IEnumerable<GameState> GenerateGameStates(int number)
        {
            var random = new Random(123);

            for (int i = 0; i < number; i++)
            {
                var board = new Board().Random(random);
                var piece = new Piece(Tetriminos.GetRandom(random));
                var nextPiece = Tetriminos.GetRandom(random);
                
                var gameState = new GameState(board, piece, nextPiece);

                yield return gameState;
            }
        }

        private Dictionary<GameState, double> BuildDictionary(IEnumerable<GameState> gameStates)
        {
            var dictionary = new Dictionary<GameState, double>();

            foreach (var gameState in gameStates)
            {
                if (!dictionary.ContainsKey(gameState))
                {
                    dictionary.Add(gameState, _heuristic.Score(gameState));
                }
            }

            return dictionary;
        }
    }
}
