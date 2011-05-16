using System;
using Encog.ML.Genetic.Genes;
using Encog.ML.Genetic.Genome;

namespace Encog.ML.Genetic.Mutate
{
    /// <summary>
    /// A simple mutation based on random numbers.
    /// </summary>
    ///
    public class MutatePerturb : IMutate
    {
        /// <summary>
        /// The amount to perturb by.
        /// </summary>
        ///
        private readonly double perturbAmount;

        /// <summary>
        /// Construct a perturb mutation.
        /// </summary>
        ///
        /// <param name="thePerturbAmount">The amount to mutate by(percent).</param>
        public MutatePerturb(double thePerturbAmount)
        {
            perturbAmount = thePerturbAmount;
        }

        #region IMutate Members

        /// <summary>
        /// Perform a perturb mutation on the specified chromosome.
        /// </summary>
        ///
        /// <param name="chromosome">The chromosome to mutate.</param>
        public void PerformMutation(Chromosome chromosome)
        {
            foreach (IGene gene  in  chromosome.Genes)
            {
                if (gene is DoubleGene)
                {
                    var doubleGene = (DoubleGene) gene;
                    double value_ren = doubleGene.Value;
                    value_ren += (perturbAmount - ((new Random()).Next()*perturbAmount*2));
                    doubleGene.Value = value_ren;
                }
            }
        }

        #endregion
    }
}