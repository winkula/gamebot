using GameBot.Core.Exceptions;
using GameBot.Game.Tetris.Searching;
using System;
using GameBot.Game.Tetris.Searching.Heuristics;

namespace GameBot.Game.Tetris.Simulator
{
    public class TetrisFitnessFunction
    {
        private const int _maxRounds = int.MaxValue;
        private const int _maxHeight = 8;
        private int _round;

        private readonly ISearch _search;
        private readonly TetrisSimulator _simulator;

        public TetrisFitnessFunction(double p1, double p2, double p3, double p4)
        {
            var heuristic = new GaHeuristic(p1, p2, p3, p4);
            _search = new SimpleSearch(heuristic);
            _simulator = new TetrisSimulator();
        }

        public double Calculate()
        {
            while (true)
            {
                try
                {
                    Update();
                    _round++;

                    if (_simulator.GameState.Board.MaximumHeight > _maxHeight)
                    {
                        // critical point reached
                        break;
                    }

                    if (_round >= _maxRounds)
                    {
                        break;
                    }
                }
                catch (GameOverException)
                {
                    break;
                }
            }
            
            return _simulator.GameState.Score / 1000000.0;
        }

        public void WriteStatus()
        {
            /*
            Console.WriteLine($@"Round {round}");
            Console.WriteLine(@"---------------------");
            Console.WriteLine($@"Level {_simulator.GameState.Level}");
            Console.WriteLine($@"Score {_simulator.GameState.Score}");
            Console.WriteLine($@"Lines {_simulator.GameState.Lines}");
            
            Console.WriteLine($"Game State:\n{_simulator.GameState}");
            */
            Console.WriteLine($@"{_simulator.GameState.Score,10} score {_round, 10} rounds");
        }

        private void Update()
        {
            var result = _search.Search(_simulator.GameState);
            if (result != null)
            {
                foreach (var move in result.Moves)
                {
                    _simulator.Simulate(move);
                }
            }
        }
    }
}