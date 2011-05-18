using Encog.MathUtil.Error;
using Encog.ML;
using Encog.ML.Data;

namespace Encog.Util.Error
{
    /// <summary>
    /// Calculate the error for regression based Machine Learning Methods.
    /// </summary>
    public class CalculateRegressionError
    {
        /// <summary>
        /// Calculate an error for a method that uses regression.
        /// </summary>
        /// <param name="method">The method to evaluate.</param>
        /// <param name="data">The training data to evaluate with.</param>
        /// <returns>The error.</returns>
        public static double CalculateError(MLRegression method,
                                            MLDataSet data)
        {
            var errorCalculation = new ErrorCalculation();

            // clear context
            if (method is MLContext)
            {
                ((MLContext) method).ClearContext();
            }


            // calculate error
            foreach (MLDataPair pair  in  data)
            {
                MLData actual = method.Compute(pair.Input);
                errorCalculation.UpdateError(actual.Data, pair.Ideal.Data);
            }
            return errorCalculation.Calculate();
        }
    }
}