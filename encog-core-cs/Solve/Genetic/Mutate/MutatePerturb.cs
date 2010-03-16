using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genes;
using Encog.MathUtil;
using Encog.Solve.Genetic.Genome;

namespace Encog.Solve.Genetic.Mutate
{
    /// <summary>
    /// A simple mutation based on random numbers.
    /// </summary>
    public class MutatePerturb : IMutate
    {
        /// <summary>
        /// The amount to perturb by.
        /// </summary>
        private double perturbAmount;

        /// <summary>
        /// Construct a perturb mutation.
        /// </summary>
        /// <param name="perturbAmount">The amount to mutate by(percent).</param>
        public MutatePerturb(double perturbAmount)
        {
            this.perturbAmount = perturbAmount;
        }


        /// <summary>
        /// Perform a perturb mutation on the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to mutate.</param>
        public void PerformMutation(Chromosome chromosome)
        {
            foreach (IGene gene in chromosome.Genes)
            {
                if (gene is DoubleGene)
                {
                    DoubleGene doubleGene = (DoubleGene)gene;
                    double value = doubleGene.Value;
                    value += (perturbAmount - (ThreadSafeRandom.NextDouble()) * perturbAmount * 2);
                    doubleGene.Value = value;
                }
            }
        }
    }
}
