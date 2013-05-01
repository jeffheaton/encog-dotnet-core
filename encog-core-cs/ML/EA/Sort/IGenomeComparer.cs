using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Sort
{
    /// <summary>
    /// Defines methods for comparing genomes. Also provides methods to apply bonuses
    /// and penalties.
    /// </summary>
    public interface IGenomeComparer : IComparer<IGenome>
    {
        /// <summary>
        /// Apply a bonus, this is a simple percent that is applied in the direction
        /// specified by the "should minimize" property of the score function.
        /// </summary>
        /// <param name="value">The current value.</param>
        /// <param name="bonus">The bonus.</param>
        /// <returns>The resulting value.</returns>
        double ApplyBonus(double value, double bonus);

        /// <summary>
        /// Apply a penalty, this is a simple percent that is applied in the
        /// direction specified by the "should minimize" property of the score
        /// function.
        /// </summary>
        /// <param name="value">The current value.</param>
        /// <param name="bonus">The penalty.</param>
        /// <returns>The resulting value.</returns>
        double ApplyPenalty(double value, double bonus);

        /// <summary>
        /// Determine if one score is better than the other.
        /// </summary>
        /// <param name="d1">The first score to compare.</param>
        /// <param name="d2">The second score to compare.</param>
        /// <returns>True if d1 is better than d2.</returns>
        bool IsBetterThan(double d1, double d2);

        /// <summary>
        /// Determine if one genome is better than the other genome.
        /// </summary>
        /// <param name="genome1">The first genome.</param>
        /// <param name="genome2">The second genome.</param>
        /// <returns>True, if genome1 is better than genome2.</returns>
        bool IsBetterThan(IGenome genome1, IGenome genome2);

        /// <summary>
        /// Returns true if the score should be minimized.
        /// </summary>
        bool ShouldMinimize { get; }
    }
}
