using Encog.ML;
using Encog.ML.Genetic.Genome;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// This adapter allows a CalculateScore object to be used to calculate a
    /// Genome's score, where a CalculateGenomeScore object would be called for.
    /// </summary>
    ///
    public class GeneticScoreAdapter : ICalculateGenomeScore
    {
        /// <summary>
        /// The calculate score object to use.
        /// </summary>
        ///
        private readonly ICalculateScore calculateScore;

        /// <summary>
        /// Construct the adapter.
        /// </summary>
        ///
        /// <param name="calculateScore_0">The CalculateScore object to use.</param>
        public GeneticScoreAdapter(ICalculateScore calculateScore_0)
        {
            calculateScore = calculateScore_0;
        }

        #region ICalculateGenomeScore Members

        /// <summary>
        /// Calculate the genome's score.
        /// </summary>
        ///
        /// <param name="genome">The genome to calculate for.</param>
        /// <returns>The calculated score.</returns>
        public double CalculateScore(IGenome genome)
        {
            var network = (MLRegression) genome.Organism;
            return calculateScore.CalculateScore(network);
        }


        /// <returns>True, if the score should be minimized.</returns>
        public bool ShouldMinimize
        {
            get { return calculateScore.ShouldMinimize; }
        }

        #endregion
    }
}