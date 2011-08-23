//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
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
        private readonly ICalculateGenomeScore _calculateScore;

        /// <summary>
        /// Construct the genome comparator.
        /// </summary>
        ///
        /// <param name="theCalculateScore">The score calculation object to use.</param>
        public GenomeComparator(ICalculateGenomeScore theCalculateScore)
        {
            _calculateScore = theCalculateScore;
        }

        /// <value>The score calculation object.</value>
        public ICalculateGenomeScore CalculateScore
        {
            get { return _calculateScore; }
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
        /// <param name="v">The current value.</param>
        /// <param name="bonus">The bonus.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyBonus(double v, double bonus)
        {
            double amount = v*bonus;
            if (_calculateScore.ShouldMinimize)
            {
                return v - amount;
            }
            return v + amount;
        }

        /// <summary>
        /// Apply a penalty, this is a simple percent that is applied in the
        /// direction specified by the "should minimize" property of the score
        /// function.
        /// </summary>
        ///
        /// <param name="v">The current value.</param>
        /// <param name="bonus">The penalty.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyPenalty(double v, double bonus)
        {
            double amount = v*bonus;
            return _calculateScore.ShouldMinimize ? v - amount : v + amount;
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
            return _calculateScore.ShouldMinimize ? Math.Min(d1, d2) : Math.Max(d1, d2);
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
            return _calculateScore.ShouldMinimize ? d1 < d2 : d1 > d2;
        }
    }
}
