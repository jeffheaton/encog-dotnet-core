using Encog.ML.Data;

namespace Encog.ML.Bayesian.Training.Search
{
    /// <summary>
    /// Search for a good Bayes structure.
    /// </summary>
    public interface IBayesSearch
    {
        /// <summary>
        /// Init the search object.
        /// </summary>
        /// <param name="theTrainer">The trainer to use.</param>
        /// <param name="theNetwork">The network to use.</param>
        /// <param name="theData">The data to use.</param>
        void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork, IMLDataSet theData);

        /// <summary>
        /// Perform an iteration. 
        /// </summary>
        /// <returns>True to continue.</returns>
        bool Iteration();
    }
}