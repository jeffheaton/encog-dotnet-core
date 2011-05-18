using Encog.ML;
using Encog.ML.Data;

namespace Encog.Util.Validate
{
    /// <summary>
    /// Perform validations on a network.
    /// </summary>
    public class ValidateNetwork
    {
        /// <summary>
        /// Validate that the specified data can be used with the method.
        /// </summary>
        /// <param name="method">The method to validate.</param>
        /// <param name="training">The training data.</param>
        public static void ValidateMethodToData(MLMethod method, MLDataSet training)
        {
            if (!(method is MLInput) || !(method is MLOutput))
            {
                throw new EncogError(
                    "This machine learning method is not compatible with the provided data.");
            }

            int trainingInputCount = training.InputSize;
            int trainingOutputCount = training.IdealSize;
            int methodInputCount = 0;
            int methodOutputCount = 0;

            if (method is MLInput)
            {
                methodInputCount = ((MLInput) method).InputCount;
            }

            if (method is MLOutput)
            {
                methodOutputCount = ((MLOutput) method).OutputCount;
            }

            if (methodInputCount != trainingInputCount)
            {
                throw new EncogError(
                    "The machine learning method has an input length of "
                    + methodInputCount + ", but the training data has "
                    + trainingInputCount + ". They must be the same.");
            }

            if (trainingOutputCount > 0 && methodOutputCount != trainingOutputCount)
            {
                throw new EncogError(
                    "The machine learning method has an output length of "
                    + methodOutputCount
                    + ", but the training data has "
                    + trainingOutputCount + ". They must be the same.");
            }
        }
    }
}