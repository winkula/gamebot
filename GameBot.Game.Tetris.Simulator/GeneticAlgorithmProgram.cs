using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using GAF;
using GAF.Operators;

namespace GameBot.Game.Tetris.Simulator
{
    public class EvaluationResult
    {
        public int Evaluations { get; private set; }
        public double Value { get; private set; }

        public EvaluationResult(double value)
        {
            Value = value;
            Evaluations = 1;
        }

        public void Add(double newValue)
        {
            if (Evaluations > 100) return;

            var oldValue = Value;
            var oldFactor = Evaluations / (Evaluations + 1.0);
            var newFactor = 1.0 / (Evaluations + 1.0);
            Value = oldValue * oldFactor + newValue * newFactor;
            Evaluations++;
        }
    }

    public class GeneticAlgorithmProgram
    {
        private Dictionary<string, EvaluationResult> _evaluations = new Dictionary<string, EvaluationResult>();
        
        public double CrossoverProbability { get; }
        public double MutationProbability { get; }
        public int ElitismPercentage { get; }
        public int ChromosomeLength { get; }
        public int PopulationSize { get; }
        public int NumGenerations { get; }
        public int MemorySize { get; }
        public int MemoryGenerationalUpdatePeriod { get; }

        private GeneticAlgorithm _ga;

        public GeneticAlgorithmProgram()
        {
            CrossoverProbability = 0.85;
            MutationProbability = 0.08;
            ElitismPercentage = 20;
            ChromosomeLength = 4 * 16;
            PopulationSize = 20;
            NumGenerations = 1000;
            MemorySize = 100;
            MemoryGenerationalUpdatePeriod = 10;
        }
        
        private void Init()
        {
            var population = new Population(
                PopulationSize,
                ChromosomeLength,
                true,
                true,
                ParentSelectionMethod.StochasticUniversalSampling,
                true);

            // create the GA itself 
            _ga = new GeneticAlgorithm(population, EvaluateFitness);
            _ga.OnGenerationComplete += GenerationComplete;

            // create the genetic operators 
            var elite = new Elite(ElitismPercentage);
            var crossover = new Crossover(CrossoverProbability, false, CrossoverType.DoublePoint);
            var mutation = new BinaryMutate(MutationProbability, false);
            var memory = new Memory(MemorySize, MemoryGenerationalUpdatePeriod);

            _ga.Operators.Add(elite);
            _ga.Operators.Add(crossover);
            _ga.Operators.Add(mutation);
            _ga.Operators.Add(memory);
        }

        public void Run()
        {
            Init();

            //run the GA 
            _ga.Run(TerminateAlgorithm);

            Console.WriteLine("=== Algorithm terminated ===");
            Console.ReadKey();
        }

        private double EvaluateFitness(Chromosome chromosome)
        {
            if (chromosome == null) throw new ArgumentNullException(nameof(chromosome));

            var parameters = GetParameters(chromosome);
            var func = new TetrisFitnessFunction(parameters[0], parameters[1], parameters[2], parameters[3]);
            double fitnessValue = func.Calculate();

            string key = chromosome.ToBinaryString();
            if (KeyExists(key))
            {
                EvaluationResult cached;
                lock (_evaluations)
                {
                    cached = _evaluations[key];
                    cached.Add(fitnessValue);
                }
                return cached.Value;
            }
            lock (_evaluations)
            {
                _evaluations.Add(key, new EvaluationResult(fitnessValue));
            }
            return fitnessValue;
        }

        private bool KeyExists(string key)
        {
            lock (_evaluations)
            {
                return _evaluations.ContainsKey(key);
            }
        }

        private double GetDouble(string binaryString)
        {
            ushort s = Convert.ToUInt16(binaryString, 2);
            if (s == ushort.MaxValue) return 1.0;
            if (s == ushort.MinValue) return 0.0;
            return (double)s / ushort.MaxValue;
        }

        private double[] GetParameters(Chromosome chromosome)
        {
            double p1 = GetDouble(chromosome.ToBinaryString(0 * 16, 16));
            double p2 = GetDouble(chromosome.ToBinaryString(1 * 16, 16));
            double p3 = GetDouble(chromosome.ToBinaryString(2 * 16, 16));
            double p4 = GetDouble(chromosome.ToBinaryString(3 * 16, 16));

            return new[] { p1, p2, p3, p4 };
        }

        private bool TerminateAlgorithm(Population population, int currentGeneration, long currentEvaluation)
        {
            return currentGeneration >= NumGenerations;
        }

        private void GenerationComplete(object sender, GaEventArgs e)
        {
            var chromosome = e.Population.GetTop(1)[0];
            //display the X, Y and fitness of the best chromosome in this generation 
            Console.WriteLine("=== Generation complete ===");
            Console.WriteLine($"Fitness: min({e.Population.MinimumFitness}), avg({e.Population.AverageFitness}), max({e.Population.MaximumFitness})");
            Console.WriteLine($"Chromosome: {chromosome.ToBinaryString()}");
            Console.WriteLine($"Parameters: {string.Join(", ", GetParameters(chromosome))}");
        }
    }
}
