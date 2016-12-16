using System;
using System.Diagnostics;
using System.Linq;
using GameBot.Game.Tetris.Data;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Searching.Heuristics;
using GAF;
using GAF.Operators;

namespace GameBot.Measurements
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Perform simple search with Yiyuan Lee's heuristic (1000)");
            Console.WriteLine($" Time: {MeasureSimpleSearch(1000).TotalMilliseconds} ms");

            Console.WriteLine("Perform predictive search with Yiyuan Lee's heuristic (100)");
            Console.WriteLine($" Time: {MeasurePredictiveSearch(100).TotalMilliseconds} ms");

            Console.WriteLine("=====");
            Console.ReadKey();
        }

        static TimeSpan MeasureSimpleSearch(int number)
        {
            var gamestates = Enumerable.Range(0, number)
                .Select(x => new Board().Random())
                .Select(x => new GameState(x, Tetriminos.GetRandom(), Tetriminos.GetRandom()))
                .ToList();

            var heuristic = new YiyuanLeeHeuristic();
            var search = new SimpleSearch(heuristic);

            var sw = new Stopwatch();
            sw.Start();

            foreach (var gamestate in gamestates)
            {
                var result = search.Search(gamestate);
            }

            sw.Stop();
            return sw.Elapsed;
        }

        static TimeSpan MeasurePredictiveSearch(int number)
        {
            var gamestates = Enumerable.Range(0, number)
                .Select(x => new Board().Random())
                .Select(x => new GameState(x, Tetriminos.GetRandom(), Tetriminos.GetRandom()))
                .ToList();

            var heuristic = new YiyuanLeeHeuristic();
            var search = new PredictiveSearch(heuristic);

            var sw = new Stopwatch();
            sw.Start();

            foreach (var gamestate in gamestates)
            {
                var result = search.Search(gamestate);
            }

            sw.Stop();
            return sw.Elapsed;
        }
    }
}
