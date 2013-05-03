using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Opp.Selection;
using Encog.ML.EA.Train;
using Encog.ML.Genetic.Genome;
using Encog.ML.EA.Genome;

namespace Encog.ML.Genetic.Mutate
{
    /// <summary>
    /// A simple mutation based on random numbers.
    /// </summary>
    public class MutatePerturb : IEvolutionaryOperator
    {
        /// <summary>
        /// The amount to perturb by.
        /// </summary>
        private double perturbAmount;

        /// <summary>
        /// Construct a perturb mutation.
        /// </summary>
        /// <param name="thePerturbAmount">The amount to mutate by(percent).</param>
        public MutatePerturb(double thePerturbAmount)
        {
            this.perturbAmount = thePerturbAmount;
        }

        /// <inheritdoc/>
        public void PerformOperation(Random rnd, IGenome[] parents, int parentIndex,
                IGenome[] offspring, int offspringIndex)
        {
            DoubleArrayGenome parent = (DoubleArrayGenome)parents[parentIndex];
            offspring[offspringIndex] = parent.Population.GenomeFactory.Factor();
            DoubleArrayGenome child = (DoubleArrayGenome)offspring[offspringIndex];

            for (int i = 0; i < parent.Size; i++)
            {
                double value = parent.Data[i];
                value += (perturbAmount - (rnd.NextDouble() * perturbAmount * 2));
                child.Data[i] = value;
            }
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
            // not needed
        }
    }
}
