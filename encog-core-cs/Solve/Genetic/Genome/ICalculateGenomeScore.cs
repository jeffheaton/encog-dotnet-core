using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Genetic.Genome
{
    /// <summary>
    /// Genetic Algorithms need a class to calculate the score.
    /// </summary>
    public interface ICalculateGenomeScore
    {
        /// <summary>
        /// Calculate this genome's score.
        /// </summary>
        /// <param name="genome">The network.</param>
        /// <returns>The score.</returns>
        double CalculateScore(IGenome genome);

        /// <summary>
        /// True if the goal is to minimize the score.
        /// </summary>
        bool ShouldMinimize { get; set; }
    }
}
