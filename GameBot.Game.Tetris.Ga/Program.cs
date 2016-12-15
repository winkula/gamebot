using System;
using System.Linq;
using GameBot.Game.Tetris.Ga.Data;
using GameBot.Game.Tetris.Ga.FitnessFunctions;
using GameBot.Game.Tetris.Ga.Operators;
using GAF;
using GAF.Operators;

namespace GameBot.Game.Tetris.Ga
{
    class Program
    {
        private static double CrossoverProbability = 0.85;
        private static double MutationProbability = 0.3;
        private static int ElitismPercentage = 10;
        private static int PopulationSize = 500;
        private static int NumGenerations = 100;
        private static bool ReevaluateAll = false;
        private static ParentSelectionMethod ParentSelectionMethod = ParentSelectionMethod.TournamentSelection;
        private static bool EvaluateInParallel = true;
        
        static void Main(string[] args)
        {
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
            var elite = new Elite(ElitismPercentage);
            var crossover = new TetrisCrossover(CrossoverProbability, ReplacementMethod.GenerationalReplacement);
            var mutation = new TetrisMutate(MutationProbability);

            ga.Operators.Add(elite);
            ga.Operators.Add(crossover);
            ga.Operators.Add(mutation);

            //run the GA 
            ga.Run(TerminateAlgorithm);

            Console.WriteLine("=== Algorithm terminated ===");
            Console.ReadKey();
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

            Console.WriteLine($"=== Generation {e.Generation} complete ===");
            Console.WriteLine($"Fitness: min({e.Population.MinimumFitness}), avg({e.Population.AverageFitness}), max({e.Population.MaximumFitness})");
            Console.WriteLine($"Chromosome: {chromosome.ToString()}");
        }
    }
}
