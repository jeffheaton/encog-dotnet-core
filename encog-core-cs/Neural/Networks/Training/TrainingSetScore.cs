using Encog.ML;
using Encog.ML.Data;
using Encog.Util.Error;

namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// Calculate a score based on a training set. This class allows simulated
    /// annealing or genetic algorithms just as you would any other training set
    /// based training method.
    /// </summary>
    ///
    public class TrainingSetScore : ICalculateScore
    {
        /// <summary>
        /// The training set.
        /// </summary>
        ///
        private readonly MLDataSet training;

        /// <summary>
        /// Construct a training set score calculation.
        /// </summary>
        ///
        /// <param name="training_0">The training data to use.</param>
        public TrainingSetScore(MLDataSet training_0)
        {
            training = training_0;
        }

        #region ICalculateScore Members

        /// <summary>
        /// Calculate the score for the network.
        /// </summary>
        ///
        /// <param name="method">The network to calculate for.</param>
        /// <returns>The score.</returns>
        public double CalculateScore(MLRegression method)
        {
            return CalculateRegressionError.CalculateError(method, training);
        }

        /// <summary>
        /// A training set based score should always seek to lower the error,
        /// as a result, this method always returns true.
        /// </summary>
        ///
        /// <returns>Returns true.</returns>
        public bool ShouldMinimize
        {
            get { return true; }
        }

        #endregion
    }
}