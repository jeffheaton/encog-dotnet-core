using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// Calculate a score based on a training set. This class allows simulated
    /// annealing or genetic algorithms just as you would any other training set
    /// based training method.
    /// </summary>
    public class TrainingSetScore
    {
        /// <summary>
        /// The training set.
        /// </summary>
        private INeuralDataSet training;

 
        /// <summary>
        /// Construct a training set score calculation. 
        /// </summary>
        /// <param name="training">The training data to use.</param>
        public TrainingSetScore(INeuralDataSet training)
        {
            this.training = training;
        }

        /// <summary>
        /// Calculate the score for the network. 
        /// </summary>
        /// <param name="network">The network to calculate for.</param>
        /// <returns>The score.</returns>
        public double CalculateScore(BasicNetwork network)
        {
            return network.CalculateError(this.training);
        }

        /// <summary>
        /// A training set based score should always seek to lower the error,
        /// as a result, this method always returns true.
        /// </summary>
        public bool ShouldMinimize
        {
            get
            {
                return true;
            }
        }
    }
}
