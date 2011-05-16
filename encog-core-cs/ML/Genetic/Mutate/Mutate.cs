using Encog.ML.Genetic.Genome;

namespace Encog.ML.Genetic.Mutate
{
    /// <summary>
    /// Defines how a chromosome is mutated.
    /// </summary>
    ///
    public interface IMutate
    {
        /// <summary>
        /// Perform a mutation on the specified chromosome.
        /// </summary>
        ///
        /// <param name="chromosome">The chromosome to mutate.</param>
        void PerformMutation(Chromosome chromosome);
    }
}