using Encog.ML.Data;
using Encog.Neural;
using Encog.Neural.Networks;

namespace Encog.Util
{
    /// <summary>
    /// Used to validate if training is valid.
    /// </summary>
    ///
    public sealed class EncogValidate
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        ///
        private EncogValidate()
        {
        }

        /// <summary>
        /// Validate a network for training.
        /// </summary>
        ///
        /// <param name="network">The network to validate.</param>
        /// <param name="training">The training set to validate.</param>
        public static void ValidateNetworkForTraining(ContainsFlat network,
                                                      MLDataSet training)
        {
            int inputCount = network.Flat.InputCount;
            int outputCount = network.Flat.OutputCount;

            if (inputCount != training.InputSize)
            {
                throw new NeuralNetworkError("The input layer size of "
                                             + inputCount + " must match the training input size of "
                                             + training.InputSize + ".");
            }

            if ((training.IdealSize > 0)
                && (outputCount != training.IdealSize))
            {
                throw new NeuralNetworkError("The output layer size of "
                                             + outputCount + " must match the training input size of "
                                             + training.IdealSize + ".");
            }
        }
    }
}