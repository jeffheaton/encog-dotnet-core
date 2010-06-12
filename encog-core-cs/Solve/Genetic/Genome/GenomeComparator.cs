// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Genetic.Genome
{
    /// <summary>
    /// Used to compare two genomes, a score object is used.
    /// </summary>
    public class GenomeComparator : IComparer<IGenome>
    {

        /// <summary>
        /// Calculate score object.
        /// </summary>
        private ICalculateGenomeScore calculateScore;

        /// <summary>
        /// Construct the genome comparator.
        /// </summary>
        /// <param name="calculateScore">The score calculation object to use.</param>
        public GenomeComparator(ICalculateGenomeScore calculateScore)
        {
            this.calculateScore = calculateScore;
        }

       
        /// <summary>
        /// Apply a bonus, this is a simple percent that is applied in the direction
        /// specified by the "should minimize" property of the score function.
        /// </summary>
        /// <param name="value">The current value.</param>
        /// <param name="bonus">The bonus.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyBonus(double value, double bonus)
        {
            double amount = value * bonus;
            if (calculateScore.ShouldMinimize)
            {
                return value - amount;
            }
            else
            {
                return value + amount;
            }
        }

        /// <summary>
        /// Apply a penalty, this is a simple percent that is applied in the
        /// direction specified by the "should minimize" property of the score
        /// function.
        /// </summary>
        /// <param name="value">The current value.</param>
        /// <param name="bonus"> The penalty.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyPenalty(double value, double bonus)
        {
            double amount = value * bonus;
            if (calculateScore.ShouldMinimize)
            {
                return value - amount;
            }
            else
            {
                return value + amount;
            }
        }

  
        /// <summary>
        /// Determine the best score from two scores, uses the "should minimize"
        /// property of the score function.
        /// </summary>
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
        /// Compare two genomes.
        /// </summary>
        /// <param name="genome1">The first genome.</param>
        /// <param name="genome2">The second genome.</param>
        /// <returns>Zero if equal, or less than or greater than zero to indicate order.</returns>
        public int Compare(IGenome genome1, IGenome genome2)
        {
            return genome1.Score.CompareTo(genome2.Score);
        }

        /// <summary>
        /// The score calculation object.
        /// </summary>
        public ICalculateGenomeScore CalculateScore
        {
            get
            {
                return calculateScore;
            }
        }

        /// <summary>
        /// Determine if one score is better than the other.
        /// </summary>
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
                return d2 > d1;
            }
        }
    }
}
