using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;
using Encog.MathUtil;

namespace Encog.Solve.Genetic.Crossover
{
    /// <summary>
    /// A simple cross over where genes are simply "spliced".
    /// Genes are allowed to repeat.
    /// </summary>
    public class Splice : ICrossover
    {
        /// <summary>
        /// The cut length.
        /// </summary>
        private int cutLength;

        /// <summary>
        /// Construct a splice cross over object.
        /// </summary>
        /// <param name="cutLength"></param>
        public Splice(int cutLength)
        {
            this.cutLength = cutLength;
        }

        /// <summary>
        /// Assuming this chromosome is the "mother" mate with the passed in
        /// "father". 
        /// </summary>
        /// <param name="mother">The mother.</param>
        /// <param name="father">The father.</param>
        /// <param name="offspring1">The first offspring.</param>
        /// <param name="offspring2">The second offspring.</param>
        public void Mate(Chromosome mother, Chromosome father,
                 Chromosome offspring1, Chromosome offspring2)
        {
            int geneLength = mother.getGenes().Count;

            // the chromosome must be cut at two positions, determine them
            int cutpoint1 = (int)(ThreadSafeRandom.NextDouble() * (geneLength - cutLength));
            int cutpoint2 = cutpoint1 + cutLength;

            // handle cut section
            for (int i = 0; i < geneLength; i++)
            {
                if (!((i < cutpoint1) || (i > cutpoint2)))
                {
                    offspring1.getGene(i).Copy(father.getGene(i));
                    offspring2.getGene(i).Copy(mother.getGene(i));
                }
            }

            // handle outer sections
            for (int i = 0; i < geneLength; i++)
            {
                if ((i < cutpoint1) || (i > cutpoint2))
                {
                    offspring1.getGene(i).Copy(mother.getGene(i));
                    offspring2.getGene(i).Copy(father.getGene(i));
                }
            }
        }
    }
}
