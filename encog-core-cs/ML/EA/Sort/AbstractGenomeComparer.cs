using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.EA.Sort
{
    /// <summary>
    /// Provides base functionality for comparing genomes. Specifically the ability
    /// to add bonuses and penalties.
    /// </summary>
    public abstract class AbstractGenomeComparer : IGenomeComparer
    {
        /// <inheritdoc/>
        public double ApplyBonus(double value, double bonus)
        {
            double amount = value * bonus;
            if (ShouldMinimize)
            {
                return value - amount;
            }
            else
            {
                return value + amount;
            }
        }

        /// <inheritdoc/>
        public double ApplyPenalty(double value, double bonus)
        {
            double amount = value * bonus;
            if (!ShouldMinimize)
            {
                return value - amount;
            }
            else
            {
                return value + amount;
            }
        }

        /// <inheritdoc/>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public bool IsBetterThan(double d1, double d2)
        {
            if (ShouldMinimize)
            {
                return d1 < d2;
            }
            else
            {
                return d1 > d2;
            }
        }



        public abstract bool IsBetterThan(Genome.IGenome genome1, Genome.IGenome genome2);

        public abstract bool ShouldMinimize
        {
            get;
        }

        /// <summary>
        /// Compare two genomes.
        /// </summary>
        /// <param name="x">The first genome.</param>
        /// <param name="y">The second genome.</param>
        /// <returns>0 if equal, <0 if x is less, >0 if y is less.</returns>
        public abstract int Compare(Genome.IGenome x, Genome.IGenome y);
    }
}
