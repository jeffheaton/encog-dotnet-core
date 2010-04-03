using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genes;
using Encog.Solve.Genetic.Genome;
using Encog.MathUtil;

namespace Encog.Solve.Genetic.Mutate
{
    /// <summary>
    /// A simple mutation where genes are shuffled.
    /// This mutation will not produce repeated genes.
    /// </summary>
    public class MutateShuffle : IMutate
    {
        /// <summary>
        /// Perform a shuffle mutation.
        /// </summary>
        /// <param name="chromosome">The chromosome to mutate.</param>
        public void PerformMutation(Chromosome chromosome)
        {
            int length = chromosome.Genes.Count;
            int iswap1 = (int)(ThreadSafeRandom.NextDouble() * length);
            int iswap2 = (int)(ThreadSafeRandom.NextDouble() * length);

            // can't be equal
            if (iswap1 == iswap2)
            {
                // move to the next, but
                // don't go out of bounds
                if (iswap1 > 0)
                {
                    iswap1--;
                }
                else
                {
                    iswap1++;
                }

            }

            // make sure they are in the right order
            if (iswap1 > iswap2)
            {
                int temp = iswap1;
                iswap1 = iswap2;
                iswap2 = temp;
            }

            IGene gene1 = chromosome.Genes[iswap1];
            IGene gene2 = chromosome.Genes[iswap2];

            // remove the two genes
            chromosome.Genes.Remove(gene1);
            chromosome.Genes.Remove(gene2);

            // insert them back in, reverse order
            chromosome.Genes.Insert(iswap1, gene2);
            chromosome.Genes.Insert(iswap2, gene1);
        }
    }
}
