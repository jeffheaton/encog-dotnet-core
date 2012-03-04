using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Bayesian.Training.Estimator
{
    /// <summary>
    /// A Bayesian estimator that does nothing.
    /// </summary>
    public class EstimatorNone : IBayesEstimator
    {
        /// <inheritdoc/>
        public void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork, Data.IMLDataSet theData)
        {
        }

        /// <inheritdoc/>
        public bool Iteration()
        {
            return false;
        }
    }
}
