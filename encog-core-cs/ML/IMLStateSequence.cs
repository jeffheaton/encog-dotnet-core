using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;

namespace Encog.ML
{
    /// <summary>
    /// A state sequence ML method, for example a Hidden Markov Model.
    /// </summary>
    public interface IMLStateSequence
    {
        /// <summary>
        /// Get the sates for the given sequence.
        /// </summary>
        /// <param name="oseq">The sequence.</param>
        /// <returns>The states.</returns>
        int[] GetStatesForSequence(IMLDataSet oseq);

        /// <summary>
        /// Determine the probability of the specified sequence.
        /// </summary>
        /// <param name="oseq">The sequence.</param>
        /// <returns>The probability.</returns>
        double Probability(IMLDataSet oseq);

        /// <summary>
        /// Determine the probability for the specified sequence and states.
        /// </summary>
        /// <param name="seq">The sequence.</param>
        /// <param name="states">The states.</param>
        /// <returns>The probability.</returns>
        double Probability(IMLDataSet seq, int[] states);
    }
}
