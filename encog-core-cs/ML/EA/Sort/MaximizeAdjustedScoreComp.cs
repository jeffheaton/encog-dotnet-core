using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Sort
{
    /// <summary>
    /// Use this comparator to maximize the adjusted score.
    /// </summary>
    [Serializable]
    public class MaximizeAdjustedScoreComp : AbstractGenomeComparer
    {
        /// <inheritdoc/>
        public override int Compare(IGenome p1, IGenome p2)
        {
            return p2.AdjustedScore.CompareTo(p1.AdjustedScore);
        }

        /// <inheritdoc/>
        public override bool IsBetterThan(IGenome prg, IGenome betterThan)
        {
            return IsBetterThan(prg.AdjustedScore,
                    betterThan.AdjustedScore);
        }

        /// <inheritdoc/>
        public override bool ShouldMinimize
        {
            get
            {
                return false;
            }
        }
    }
}
