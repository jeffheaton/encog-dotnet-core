using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Sort
{
    /// <summary>
    /// Use this comparator to minimize the score.
    /// </summary>
    [Serializable]
    public class MinimizeScoreComp : AbstractGenomeComparer
    {

        /// <inheritdoc/>
        public override int Compare(IGenome p1, IGenome p2)
        {
            return p2.Score.CompareTo(p1.Score);
        }

        /// <inheritdoc/>
        public bool IsBetterThan(IGenome prg, IGenome betterThan)
        {
            return IsBetterThan(prg.AdjustedScore,
                    betterThan.AdjustedScore);
        }

        /// <inheritdoc/>
        public bool ShouldMinimize
        {
            get
            {
                return false;
            }
        }
    }
}
