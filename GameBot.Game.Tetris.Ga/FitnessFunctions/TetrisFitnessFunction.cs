using System.Collections.Generic;
using System.Linq;
using GameBot.Core.Exceptions;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Searching.Heuristics;
using GameBot.Game.Tetris.Simulators;
using GAF;

namespace GameBot.Game.Tetris.Ga.FitnessFunctions
{
    public class TetrisFitnessFunction : IFitness
    {
        private readonly int _games;
        private readonly int _pieces;

        public TetrisFitnessFunction(int games, int pieces)
        {
            _games = games;
            _pieces = pieces;
        }

        public double EvaluateFitness(Chromosome chromosome)
        {
            var p1 = chromosome.Genes[0].RealValue;
            var p2 = chromosome.Genes[1].RealValue;
            var p3 = chromosome.Genes[2].RealValue;
            var p4 = chromosome.Genes[3].RealValue;

            var heuristic = new GaHeuristic(p1, p2, p3, p4);
            var search = new SimpleSearch(heuristic);
            var maxHeight = SimulateAvg(search, _games, _pieces);
            
            return (20.0 - maxHeight) / 20.0;
        }

        private double SimulateAvg(ISearch search, int games, int pieces)
        {
            var list = new List<double>();
            for (int i = 0; i < games; i++)
            {
                list.Add(Simulate(search, pieces));
            }
            return list.Average();
        }

        private double Simulate(ISearch search, int pieces)
        {
            var simulator = new TetrisSimulator();

            int round = 0;
            int maxHeight = 0;
            while (true)
            {
                try
                {
                    // update
                    var result = search.Search(simulator.GameState);
                    if (result == null) throw new GameOverException();

                    foreach (var move in result.Moves)
                    {
                        simulator.Simulate(move);
                    }

                    round++;

                    maxHeight = System.Math.Max(simulator.GameState.Board.MaximumHeight, maxHeight);
                    
                    if (round >= pieces)
                    {
                        break;
                    }
                }
                catch (GameOverException)
                {
                    break;
                }
            }

            //return simulator.GameState.Lines;
            return maxHeight;
        }
    }
}