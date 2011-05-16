using Encog.ML.Data;

namespace Encog.ML
{
    /// <summary>
    /// Defines Machine Learning Method that can calculate an error based on a 
    /// data set.
    /// </summary>
    ///
    public interface MLError : MLMethod
    {
        /// <summary>
        /// Calculate the error of the ML method, given a dataset.
        /// </summary>
        ///
        /// <param name="data">The dataset.</param>
        /// <returns>The error.</returns>
        double CalculateError(MLDataSet data);
    }
}