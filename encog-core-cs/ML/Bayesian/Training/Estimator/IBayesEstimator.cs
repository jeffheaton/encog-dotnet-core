using Encog.ML.Data;

namespace Encog.ML.Bayesian.Training.Estimator
{
    /// <summary>
    /// An estimator is used during Bayesian training to determine optimal probability values.
    /// </summary>
    public interface IBayesEstimator
    {
        /// <summary>
        /// Init the estimator.
        /// </summary>
        /// <param name="theTrainer">The trainer.</param>
        /// <param name="theNetwork">The network.</param>
        /// <param name="theData">The data.</param>
        void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork, IMLDataSet theData);

        /// <summary>
        /// Perform an iteration.
        /// </summary>
        /// <returns>True, if we should contune.</returns>
        bool Iteration();
    }
}