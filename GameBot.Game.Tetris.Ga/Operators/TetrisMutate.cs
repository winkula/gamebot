using GameBot.Game.Tetris.Ga.Data;
using GAF;
using GAF.Operators;
using GAF.Threading;

namespace GameBot.Game.Tetris.Ga.Operators
{
    public class TetrisMutate : MutateBase
    {
        private double _mutationFactorMax = 0.2;

        public TetrisMutate(double mutationProbability) : base(mutationProbability)
        {
            Enabled = true;
            RequiresEvaluatedPopulation = false;
        }

        protected override void Mutate(Chromosome child)
        {
            base.Mutate(child);

            var vec = new Vector4(child).Normalize();
            child.Genes.Clear();
            child.Genes.AddRange(vec.ToChromosome().Genes);
        }

        protected override void MutateGene(Gene gene)
        {
            var random = RandomProvider.GetThreadRandom();

            var delta = (random.NextDouble() - 0.5) * 2.0 * _mutationFactorMax;

            double oldValue = gene.RealValue;
            gene.ObjectValue = oldValue + delta;
        }
    }
}
