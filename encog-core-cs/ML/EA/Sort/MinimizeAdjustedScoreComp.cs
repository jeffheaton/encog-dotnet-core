using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Sort
{
    /// <summary>
    /// Use this comparator to minimize the adjusted score.
    /// </summary>
    [Serializable]
    public class MinimizeAdjustedScoreComp : AbstractGenomeComparer
    {
        /// <inheritdoc/>
        public override int Compare(IGenome p1, IGenome p2)
        {
            return p1.AdjustedScore.CompareTo(p2.AdjustedScore);
        }

        /// <inheritdoc/>
        public bool IsBetterThan(IGenome prg, IGenome betterThan)
        {
            return IsBetterThan(prg.AdjustedScore,
                    betterThan.AdjustedScore);
        }

        /// <inheritdoc/>
        public bool shouldMinimize()
        {
            return true;
        }
    }
}
