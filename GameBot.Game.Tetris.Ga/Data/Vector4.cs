using System;
using GAF;

namespace GameBot.Game.Tetris.Ga.Data
{
    public class Vector4
    {
        public double W { get; }
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public double Length => System.Math.Sqrt(W * W + X * X + Y * Y + Z * Z);



        public Vector4(Chromosome chromosome)
        {
            W = chromosome.Genes[0].RealValue;
            X = chromosome.Genes[1].RealValue;
            Y = chromosome.Genes[2].RealValue;
            Z = chromosome.Genes[3].RealValue;
        }

        public Vector4(Random random)
        {
            W = (random.NextDouble() - 0.5) * 2;
            X = (random.NextDouble() - 0.5) * 2;
            Y = (random.NextDouble() - 0.5) * 2;
            Z = (random.NextDouble() - 0.5) * 2;
        }

        public Vector4() : this(0.0, 0.0, 0.0, 0.0)
        {
        }

        public Vector4(Vector4 other) : this(other.W, other.X, other.Y, other.Z)
        {
        }

        public Vector4(double w, double x, double y, double z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }

        public Vector4 Add(Vector4 other)
        {
            return new Vector4(
                W + other.W,
                X + other.X,
                Y + other.Y,
                Z + other.Z);
        }

        public Vector4 Multiply(double factor)
        {
            return new Vector4(
                W * factor,
                X * factor,
                Y * factor,
                Z * factor);
        }

        public Vector4 Normalize()
        {
            var length = Length;
            if (length == 0.0) return new Vector4(0.5, 0.5, 0.5, 0.5);

            var factor = 1.0 / length;
            return Multiply(factor);
        }

        public Chromosome ToChromosome()
        {
            var chromosome = new Chromosome();

            chromosome.Genes.Add(new Gene(W));
            chromosome.Genes.Add(new Gene(X));
            chromosome.Genes.Add(new Gene(Y));
            chromosome.Genes.Add(new Gene(Z));

            return chromosome;
        }
    }
}
