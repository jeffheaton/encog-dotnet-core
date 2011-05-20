using System;
using System.Collections.Generic;

namespace Encog.ML.Genetic.Genome
{
    /// <summary>
    /// Used to compare two genomes, a score object is used.
    /// </summary>
    ///
    public class GenomeComparator : IComparer<IGenome>
    {
        /// <summary>
        /// The method to calculate the score.
        /// </summary>
        ///
        private readonly ICalculateGenomeScore calculateScore;

        /// <summary>
        /// Construct the genome comparator.
        /// </summary>
        ///
        /// <param name="theCalculateScore">The score calculation object to use.</param>
        public GenomeComparator(ICalculateGenomeScore theCalculateScore)
        {
            calculateScore = theCalculateScore;
        }

        /// <value>The score calculation object.</value>
        public ICalculateGenomeScore CalculateScore
        {
            get { return calculateScore; }
        }

        #region IComparer<IGenome> Members

        /// <summary>
        /// Compare two genomes.
        /// </summary>
        ///
        /// <param name="genome1">The first genome.</param>
        /// <param name="genome2">The second genome.</param>
        /// <returns>Zero if equal, or less than or greater than zero to indicate
        /// order.</returns>
        public int Compare(IGenome genome1, IGenome genome2)
        {
            return genome1.Score.CompareTo(genome2.Score);
        }

        #endregion

        /// <summary>
        /// Apply a bonus, this is a simple percent that is applied in the direction
        /// specified by the "should minimize" property of the score function.
        /// </summary>
        ///
        /// <param name="value_ren">The current value.</param>
        /// <param name="bonus">The bonus.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyBonus(double value_ren, double bonus)
        {
            double amount = value_ren*bonus;
            if (calculateScore.ShouldMinimize)
            {
                return value_ren - amount;
            }
            else
            {
                return value_ren + amount;
            }
        }

        /// <summary>
        /// Apply a penalty, this is a simple percent that is applied in the
        /// direction specified by the "should minimize" property of the score
        /// function.
        /// </summary>
        ///
        /// <param name="value_ren">The current value.</param>
        /// <param name="bonus">The penalty.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyPenalty(double value_ren, double bonus)
        {
            double amount = value_ren*bonus;
            if (calculateScore.ShouldMinimize)
            {
                return value_ren - amount;
            }
            else
            {
                return value_ren + amount;
            }
        }

        /// <summary>
        /// Determine the best score from two scores, uses the "should minimize"
        /// property of the score function.
        /// </summary>
        ///
        /// <param name="d1">The first score.</param>
        /// <param name="d2">The second score.</param>
        /// <returns>The best score.</returns>
        public double BestScore(double d1, double d2)
        {
            if (calculateScore.ShouldMinimize)
            {
                return Math.Min(d1, d2);
            }
            else
            {
                return Math.Max(d1, d2);
            }
        }


        /// <summary>
        /// Determine if one score is better than the other.
        /// </summary>
        ///
        /// <param name="d1">The first score to compare.</param>
        /// <param name="d2">The second score to compare.</param>
        /// <returns>True if d1 is better than d2.</returns>
        public bool IsBetterThan(double d1, double d2)
        {
            if (calculateScore.ShouldMinimize)
            {
                return d1 < d2;
            }
            else
            {
                return d1 > d2;
            }
        }
    }
}