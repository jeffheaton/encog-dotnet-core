using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;

namespace Encog.ML.Bayesian.Training.Search
{
    /// <summary>
    /// Simple class to perform no search for optimal network structure.
    /// </summary>
    public class SearchNone : IBayesSearch
    {
        /// <inheritdoc/>
        public void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork,
                IMLDataSet theData)
        {

        }

        /// <inheritdoc/>
        public bool Iteration()
        {
            return false;

        }
    }
}
