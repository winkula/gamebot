using System;
using System.Collections.Generic;
using System.Linq;
using GameBot.Game.Tetris.Ga.Data;
using GAF;
using GAF.Extensions;
using GAF.Operators;
using GAF.Threading;

namespace GameBot.Game.Tetris.Ga.Operators
{
    public class TetrisCrossover : OperatorBase, IGeneticOperator
    {
        private readonly object _syncLock = new object();

        /// <summary>
        /// Delegage definition for the CrossoverComplete event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void CrossoverCompleteHandler(object sender, CrossoverEventArgs e);

        private double _crossoverProbabilityS = 1.0;
        private int _evaluations;
        private bool _allowDuplicatesS;
        private ReplacementMethod _replacementMethodS;
        private int _currentPopulationSize;
        private int _numberOfChildrenToGenerate;

        public TetrisCrossover(double crossOverProbability, ReplacementMethod replacementMethod)
        {
            CrossoverProbability = crossOverProbability;
            AllowDuplicates = true;
            ReplacementMethod = replacementMethod;
            RequiresEvaluatedPopulation = true;
            
            Enabled = true;
        }

        /// <summary>
        /// This is the method that invokes the operator. This should not normally be called explicitly.
        /// </summary>
        /// <param name="currentPopulation"></param>
        /// <param name="newPopulation"></param>
        /// <param name="fitnessFunctionDelegate"></param>
        public new virtual void Invoke(Population currentPopulation, ref Population newPopulation, FitnessFunction fitnessFunctionDelegate)
        {
            if (currentPopulation.Solutions == null || currentPopulation.Solutions.Count == 0)
            {
                throw new ArgumentException("There are no Solutions in the current Population.");
            }

            if (newPopulation == null)
            {
                newPopulation = currentPopulation.CreateEmptyCopy();
            }

            CurrentPopulation = currentPopulation;
            NewPopulation = newPopulation;
            FitnessFunction = fitnessFunctionDelegate;

            if (!Enabled)
                return;

            //need to store this as we cannot handle a change until once this generation has started
            _replacementMethodS = ReplacementMethod;

            this.Process();

            //TODO: Test this, we shouldn't need it.
            newPopulation = NewPopulation;
        }

        /// <summary>
        /// Returns the number of evaluations that were undertaken as part of the operators Invocation.
        /// For example the 'Steady State' reproduction method, compares new childrem with those already
        /// in the population and therefore performs the analysis as part of the operators invocation.
        /// </summary>
        /// <returns></returns>
        public new int GetOperatorInvokedEvaluations()
        {
            return _evaluations;
        }

        /// <summary>
        /// Main process loop for performing a crossover on a population.
        /// </summary>
        protected void Process()
        {

            int maxLoop = 100;
            int eliteCount = 0;

            //reset the number of evaluations
            _evaluations = 0;

            //now that we know how many children we are to be creating, in the case of 'Delete Last'
            //we copy the current generation across and add new children where they are greater than
            //the worst solution.
            if (_replacementMethodS == ReplacementMethod.DeleteLast)
            {
                //copy everything accross including the elites
                NewPopulation.Solutions.Clear();
                NewPopulation.Solutions.AddRange(CurrentPopulation.Solutions);
            }
            else
            {
                //just copy the elites, this will take all elites

                //TODO: Sort out what we do if we overfill the population with elites
                var elites = CurrentPopulation.GetElites();
                eliteCount = elites.Count();
                if (elites != null && eliteCount > 0)
                {
                    NewPopulation.Solutions.AddRange(elites);
                }
            }
            _currentPopulationSize = CurrentPopulation.Solutions.Count;

            _numberOfChildrenToGenerate =
                _currentPopulationSize - eliteCount;

            while (_numberOfChildrenToGenerate > 0)
            {
                //emergency exit
                if (maxLoop <= 0)
                {
                    throw new ChromosomeException("Unable to create a suitable child. If duplicates have been disallowed then consider increasing the chromosome length or increasing the number of elites.");
                }

                //these will hold the children
                Chromosome c1 = null;
                Chromosome c2 = null;

                //select some parents
                var parents = CurrentPopulation.SelectParents();
                var p1 = parents[0];
                var p2 = parents[1];

                //crossover
                PerformCrossover(p1, p2, CrossoverProbability, out c1, out c2);

                if (AddChild(c1))
                {
                    _numberOfChildrenToGenerate--;
                }
                else
                {
                    //unable to create child
                    maxLoop--;
                }

                //see if we can add the secomd
                if (_numberOfChildrenToGenerate > 0)
                {
                    if (AddChild(c2))
                    {
                        _numberOfChildrenToGenerate--;
                    }
                    else
                    {
                        //unable to create child
                        maxLoop--;
                    }
                }
            }
        }

        #region Private Methods

        internal void PerformCrossover(Chromosome p1, Chromosome p2, double crossoverProbability, out Chromosome c1, out Chromosome c2)
        {
            var chromosomeLength = p1.Genes.Count;

            if (chromosomeLength != p2.Genes.Count)
            {
                throw new ArgumentException("Parent chromosomes are not the same length.");
            }

            List<Gene> cg1 = new List<Gene>();
            List<Gene> cg2 = new List<Gene>();

            var rd = RandomProvider.GetThreadRandom().NextDouble();
            if (rd <= crossoverProbability)
            {
                var fitness1 = p1.Fitness;
                var fitness2 = p2.Fitness;

                var fitness1Normlized = fitness1 / (fitness1 + fitness2);
                var fitness2Normlized = fitness2 / (fitness1 + fitness2);

                var vect1 = new Vector4(p1).Normalize();
                var vect2 = new Vector4(p2).Normalize();

                var byFitness = vect1.Multiply(fitness1Normlized).Add(vect2.Multiply(fitness2Normlized)).Normalize();
                var fair = vect1.Multiply(0.5).Add(vect2.Multiply(0.5)).Normalize();

                cg1.AddRangeCloned(byFitness.ToChromosome().Genes);
                cg2.AddRangeCloned(fair.ToChromosome().Genes);
            }
            else
            {
                cg1.AddRangeCloned(p1.Genes);
                cg2.AddRangeCloned(p2.Genes);
            }

            if (cg1.Count != chromosomeLength || cg1.Count != chromosomeLength)
            {
                throw new ChromosomeCorruptException("Chromosome is corrupt!");
            }

            c1 = new Chromosome(cg1);
            c2 = new Chromosome(cg2);
        }

        /// <summary>
        /// Adds a child to the new population depending upon the criteria set in relation to replacement
        /// method and duplicate handling. The method updates the evaluation count and returns true if a 
        /// child was added to the new population.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private bool AddChild(Chromosome child)
        {
            var result = false;

            if (_replacementMethodS == ReplacementMethod.DeleteLast)
            {
                child.Evaluate(FitnessFunction);
                _evaluations++;

                if (child.Genes != null && child.Fitness > CurrentPopulation.MinimumFitness)
                {
                    //add the child if there is still space
                    if (AllowDuplicates || !NewPopulation.SolutionExists(child))
                    {
                        //add the new child and remove the last
                        NewPopulation.Solutions.Add(child);
                        if (NewPopulation.Solutions.Count > _currentPopulationSize)
                        {
                            NewPopulation.Solutions.Sort();
                            NewPopulation.Solutions.RemoveAt(_currentPopulationSize - 1);
                            result = true;
                        }
                        else
                        {
                            //we return true whether we actually added or not what we are effectively
                            //doing here is adding the original child from the current solution
                            result = true;
                        }
                    }
                }
            }
            else
            {
                //we need to cater for the user switching from delete last to Generational Replacement
                //in this scenrio we will have a full population but with still some children to generate
                if (NewPopulation.Solutions.Count + _numberOfChildrenToGenerate > _currentPopulationSize)
                {
                    //assume all done for this generation
                    _numberOfChildrenToGenerate = 0;
                    return false;
                }

                if (child.Genes != null)
                {
                    //add the child if there is still space
                    if (this.AllowDuplicates || !NewPopulation.SolutionExists(child))
                    {
                        NewPopulation.Solutions.Add(child);
                        result = true;
                    }
                }
            }
            return result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Sets/Gets whether duplicates are allowed in the population. 
        /// The setting and getting of this property is thread safe.
        /// </summary>
        public bool AllowDuplicates
        {
            get
            {
                lock (_syncLock)
                {
                    return _allowDuplicatesS;
                }
            }
            set
            {
                lock (_syncLock)
                {
                    _allowDuplicatesS = value;
                }
            }
        }

        /// <summary>
        /// Sets/Gets the method used for the deletion of chromosomes from the population.
        /// The setting and getting of this property is thread safe.
        /// </summary>
        public ReplacementMethod ReplacementMethod
        {
            set
            {
                lock (_syncLock)
                {
                    _replacementMethodS = value;
                }
            }
            get
            {
                lock (_syncLock)
                {
                    return _replacementMethodS;
                }
            }
        }

        /// <summary>
        /// Sets/gets the current crossover probability. 
        /// The setting and getting of this property is thread safe.
        /// </summary>
        public double CrossoverProbability
        {
            get
            {
                lock (_syncLock)
                {
                    return _crossoverProbabilityS;
                }
            }
            set
            {
                lock (_syncLock)
                {
                    _crossoverProbabilityS = value;
                }
            }
        }

        #endregion
    }
}
