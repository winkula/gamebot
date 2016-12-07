using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Quantizers;
using GameBot.Game.Tetris.Agents;
using GameBot.Game.Tetris.Agents.States;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Extraction.Extractors;
using GameBot.Game.Tetris.Extraction.Matchers;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Searching.Heuristics;
using GameBot.Test.Extensions;
using Moq;
using NUnit.Framework;

namespace GameBot.Test.Game.Tetris
{
    [TestFixture]
    public class AgentTests
    {
        private const int _numSamples = 3;

        private Mock<IConfig> _configMock;
        private Mock<IClock> _clockMock;
        private Mock<IExecutor> _executorMock;

        private IQuantizer _quantizer;
        private IExtractor _extractor;
        private IBoardExtractor _boardExtractor;
        private ISearch _search;

        private IScreenshot _screenshot;
        private TetrisAgent _agent;
        private TetrisAnalyzeState _analyzeState;
        private TetrisExecuteState _executeState;

        [TestFixtureSetUp]
        public void InitOnce()
        {
            _configMock = TestHelper.GetFakeConfig();
            _configMock.ConfigValue("Game.Tetris.Extractor.Samples", _numSamples);
            _configMock.ConfigValue("Game.Tetris.Timing.AnalyzePaddingTime", 0);
            _configMock.ConfigValue("Game.Tetris.Timing.DropPaddingTime", 0);

            _quantizer = new MorphologyQuantizer(_configMock.Object);
            _extractor = new MorphologyExtractor(_configMock.Object);
            _boardExtractor = new BoardExtractor(new MorphologyMatcher());
            _search = new SimpleSearch(new YiyuanLeeHeuristic());

            _clockMock = new Mock<IClock>();
            _executorMock = new Mock<IExecutor>();
        }

        [SetUp]
        public void Init()
        {
            _screenshot = new EmguScreenshot("Screenshots/tetris_play_1.png", TimeSpan.Zero);
            _screenshot.OriginalImage = _screenshot.Image;

            var currentPiece = Tetrimino.S;
            var nextPiece = Tetrimino.L;
            var moves = new List<Move> { Move.Left, Move.Rotate, Move.Drop };

            _agent = new TetrisAgent(_configMock.Object, _clockMock.Object, _quantizer, _extractor, _boardExtractor, _search);
            _agent.GameState = new GameState(currentPiece, nextPiece);

            _analyzeState = new TetrisAnalyzeState(_agent, currentPiece);
            _executeState = new TetrisExecuteState(_agent, moves, new Piece(currentPiece));
        }

        [Test]
        public void TestAnalyzeAndExecute()
        {
            _agent.SetState(_analyzeState);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < _numSamples / 2 + 1; i++)
            {
                _agent.Extract(_screenshot);
                _agent.Play(_executorMock.Object);
            }

            stopwatch.Stop();
            Debug.WriteLine($"Elapsed TestAnalyzeAndExecute: {stopwatch.ElapsedMilliseconds} ms");
        }

        [Test]
        public void TestExecute()
        {
            _agent.SetState(_executeState);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _agent.Extract(_screenshot);
            _agent.Play(_executorMock.Object);

            stopwatch.Stop();
            Debug.WriteLine($"Elapsed TestExecute: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
