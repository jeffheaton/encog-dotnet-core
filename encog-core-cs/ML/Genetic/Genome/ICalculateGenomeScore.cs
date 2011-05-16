namespace Encog.ML.Genetic.Genome
{
    /// <summary>
    /// Genetic Algorithms need a class to calculate the score.
    /// </summary>
    ///
    public interface ICalculateGenomeScore
    {
        /// <returns>True if the goal is to minimize the score.</returns>
        bool ShouldMinimize { get; }

        /// <summary>
        /// Calculate this genome's score.
        /// </summary>
        ///
        /// <param name="genome">The genome.</param>
        /// <returns>The score.</returns>
        double CalculateScore(IGenome genome);
    }
}