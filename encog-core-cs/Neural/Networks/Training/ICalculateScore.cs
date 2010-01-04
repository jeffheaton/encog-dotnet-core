using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// Used by simulated annealing and genetic algorithms to calculate the score
    /// for a neural network.  This allows networks to be ranked.  We may be seeking
    /// a high or a low score, depending on the value the shouldMinimize
    /// method returns.
    /// </summary>
    public interface ICalculateScore
    {
        /// <summary>
        /// Calculate this network's score.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <returns>The score.</returns>
        double CalculateScore(BasicNetwork network);

        /// <summary>
        /// Specifies if the score should be minimized.
        /// </summary>
        bool ShouldMinimize
        {
            get;
        }
    }
}
