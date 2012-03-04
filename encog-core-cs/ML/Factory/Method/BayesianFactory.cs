using System;
using Encog.ML.Bayesian;

namespace Encog.ML.Factory.Method
{
    /// <summary>
    /// Factory to create Bayesian networks
    /// </summary>
    public class BayesianFactory
    {
        /// <summary>
        /// Create a bayesian network.
        /// </summary>
        /// <param name="architecture">The architecture to use.</param>
        /// <param name="input">The input neuron count.</param>
        /// <param name="output">The output neuron count.</param>
        /// <returns>The new bayesian network.</returns>
        public IMLMethod Create(String architecture, int input,
                                int output)
        {
            var method = new BayesianNetwork {Contents = architecture};
            return method;
        }
    }
}