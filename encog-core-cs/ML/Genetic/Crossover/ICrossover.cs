using Encog.ML.Genetic.Genome;

namespace Encog.ML.Genetic.Crossover
{
    /// <summary>
    /// Specifies how "crossover" or mating happens.
    /// </summary>
    ///
    public interface ICrossover
    {
        /// <summary>
        /// Mate two chromosomes.
        /// </summary>
        ///
        /// <param name="mother">The mother.</param>
        /// <param name="father">The father.</param>
        /// <param name="offspring1">The first offspring.</param>
        /// <param name="offspring2">The second offspring.</param>
        void Mate(Chromosome mother, Chromosome father,
                  Chromosome offspring1, Chromosome offspring2);
    }
}