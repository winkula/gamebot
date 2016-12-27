using System;
using System.IO;
using System.Linq;
using GameBot.Game.Tetris.Ga.Data;
using GameBot.Game.Tetris.Ga.FitnessFunctions;
using GameBot.Game.Tetris.Ga.Operators;
using GAF;
using GAF.Operators;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace GameBot.Game.Tetris.Ga
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static double CrossoverProbability = 0.3;
        private static double MutationProbability = 0.02;
        //private static int ElitismPercentage = 5;
        private static int PopulationSize = 100;
        private static int NumGenerations = 100;
        private static bool ReevaluateAll = false;
        private static ParentSelectionMethod ParentSelectionMethod = ParentSelectionMethod.TournamentSelection;
        private static bool EvaluateInParallel = true;
        
        static void Main(string[] args)
        {
            ConfigureLogging();

            var population = new Population(
                0,
                4,
                ReevaluateAll,
                true,
                ParentSelectionMethod,
                EvaluateInParallel);
            Populate(population);
            
            // create the GA itself 
            var ga = new GeneticAlgorithm(population, EvaluateFitness);
            ga.OnGenerationComplete += GenerationComplete;

            // create the genetic operators 
            //var elite = new Elite(ElitismPercentage);
            var crossover = new TetrisCrossover(CrossoverProbability, ReplacementMethod.DeleteLast);
            var mutation = new TetrisMutate(MutationProbability);

            //ga.Operators.Add(elite);
            ga.Operators.Add(crossover);
            ga.Operators.Add(mutation);

            //run the GA 
            ga.Run(TerminateAlgorithm);

            _logger.Info("=== Algorithm terminated ===");
            Console.WriteLine("=== Algorithm terminated ===");
        }

        static void Populate(Population population)
        {
            var r = new Random();
            for (int i = 0; i < PopulationSize; i++)
            { 
                population.Solutions.Add(new Vector4(r).Normalize().ToChromosome());
            }
        }

        static double EvaluateFitness(Chromosome chromosome)
        {
            return new TetrisFitnessFunction(100, 100).EvaluateFitness(chromosome);
        }

        static bool TerminateAlgorithm(Population population, int currentGeneration, long currentEvaluation)
        {
            return currentGeneration >= NumGenerations;
        }

        static void GenerationComplete(object sender, GaEventArgs e)
        {
            var chromosome = e.Population.GetTop(1).First();

            _logger.Info($"> Generation {e.Generation} complete");
            _logger.Info($"  Fitness: min({e.Population.MinimumFitness}), avg({e.Population.AverageFitness}), max({e.Population.MaximumFitness})");
            _logger.Info($"  Chromosome: {chromosome.ToString()}");

            Console.WriteLine($"=== Generation {e.Generation} complete ===");
            Console.WriteLine($"Fitness: min({e.Population.MinimumFitness}), avg({e.Population.AverageFitness}), max({e.Population.MaximumFitness})");
            Console.WriteLine($"Chromosome: {chromosome.ToString()}");
        }
        
        static void ConfigureLogging()
        {
            var config = new LoggingConfiguration();
            
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "GameBot_Genetic.txt");
            var fileTarget = new FileTarget();
            fileTarget.Layout = @"${message}";
            fileTarget.FileName = path;
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));

            LogManager.Configuration = config;
        }
    }
}
