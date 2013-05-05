using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Opp.Selection;
using Encog.ML.EA.Train;
using Encog.ML.EA.Genome;
using Encog.ML.Genetic.Genome;
using Encog.MathUtil.Randomize;

namespace Encog.ML.Genetic.Mutate
{
    /// <summary>
    /// A simple mutation where genes are shuffled.
    /// This mutation will not produce repeated genes.
    /// </summary>
    public class MutateShuffle : IEvolutionaryOperator
    {
        /// <summary>
        /// The owner.
        /// </summary>
        private IEvolutionaryAlgorithm owner;

        /// <inheritdoc/>
        public void PerformOperation(EncogRandom rnd, IGenome[] parents, int parentIndex,
                IGenome[] offspring, int offspringIndex)
        {
            IArrayGenome parent = (IArrayGenome)parents[parentIndex];
            offspring[offspringIndex] = this.owner.Population.GenomeFactory.Factor();
            IArrayGenome child = (IArrayGenome)offspring[offspringIndex];

            child.Copy(parent);

            int length = parent.Size;
            int iswap1 = (int)(rnd.NextDouble() * length);
            int iswap2 = (int)(rnd.NextDouble() * length);

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

            child.Swap(iswap1, iswap2);
        }

        /// <summary>
        /// The number of offspring produced, which is 1 for this mutation.
        /// </summary>
        public int OffspringProduced
        {
            get
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public int ParentsNeeded
        {
            get
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public void Init(IEvolutionaryAlgorithm theOwner)
        {
            this.owner = theOwner;
        }
    }
}
