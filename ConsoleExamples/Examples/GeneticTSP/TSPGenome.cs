using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;
using Encog.Examples.Util;
using Encog.Solve.Genetic;
using Encog.MathUtil;
using Encog.Solve.Genetic.Genes;

namespace Encog.Examples.GeneticTSP
{
    public class TSPGenome : BasicGenome
    {
        private Chromosome pathChromosome;

        public TSPGenome(GeneticAlgorithm owner, City[] cities)
            : base(owner)
        {

            int[] organism = new int[cities.Length];
            bool[] taken = new bool[cities.Length];

            for (int i = 0; i < organism.Length; i++)
            {
                taken[i] = false;
            }
            for (int i = 0; i < organism.Length - 1; i++)
            {
                int icandidate;
                do
                {
                    icandidate = (int)(ThreadSafeRandom.NextDouble() * organism.Length);
                } while (taken[icandidate]);
                organism[i] = icandidate;
                taken[icandidate] = true;
                if (i == organism.Length - 2)
                {
                    icandidate = 0;
                    while (taken[icandidate])
                    {
                        icandidate++;
                    }
                    organism[i + 1] = icandidate;
                }
            }

            this.pathChromosome = new Chromosome();
            this.Chromosomes.Add(this.pathChromosome);

            for (int i = 0; i < organism.Length; i++)
            {
                IntegerGene gene = new IntegerGene();
                gene.Value = organism[i];
                this.pathChromosome.Genes.Add(gene);
            }
            Organism = organism;

            Encode();

        }

        public override void Decode()
        {
            Chromosome chromosome = this.Chromosomes[0];
            int[] organism = new int[chromosome.Genes.Count];

            for (int i = 0; i < chromosome.Genes.Count; i++)
            {
                IntegerGene gene = (IntegerGene)chromosome.Genes[i];
                organism[i] = gene.Value;
            }

            Organism = organism;
        }

        public override void Encode()
        {
            Chromosome chromosome = this.Chromosomes[0];

            int[] organism = (int[])this.Organism;

            for (int i = 0; i < chromosome.Genes.Count; i++)
            {
                IntegerGene gene = (IntegerGene)chromosome.Genes[i];
                gene.Value = organism[i];
            }
        }
    }
}
