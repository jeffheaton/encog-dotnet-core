using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// An adapter that allows a ICalculateScore object to calculate the 
    /// score for a Genome.  This can be very useful for training
    /// neural networks with a Genetic algorithm.
    /// </summary>
    public class GeneticScoreAdapter : ICalculateGenomeScore
    {
        /// <summary>
        /// The object used to calculate the score.
        /// </summary>
        private ICalculateScore calculateScore;

        /// <summary>
        /// Construct a genetic score adapter.
        /// </summary>
        /// <param name="calculateScore">The ICalculateScore object to use.</param>
        public GeneticScoreAdapter(ICalculateScore calculateScore)
        {
            this.calculateScore = calculateScore;
        }

        /// <summary>
        /// Calculate the score for a genome.
        /// </summary>
        /// <param name="genome">The genome to calculate the score for.</param>
        /// <returns>The calculated score.</returns>
        public double CalculateScore(IGenome genome)
        {
            BasicNetwork network = (BasicNetwork)genome.Organism;
            return this.calculateScore.CalculateScore(network);
        }

        /// <summary>
        /// Should the score be minimized.
        /// </summary>
        public bool ShouldMinimize
        {
            get
            {
                return this.calculateScore.ShouldMinimize;
            }
        }
    }
}
