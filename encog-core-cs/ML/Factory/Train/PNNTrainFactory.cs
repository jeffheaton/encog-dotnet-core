using System;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.PNN;
using Encog.Neural.PNN;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// A factory used to create PNN trainers. 
    /// </summary>
    ///
    public class PNNTrainFactory
    {
        /// <summary>
        /// Create a PNN trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public MLTrain Create(MLMethod method,
                              MLDataSet training, String args)
        {
            if (!(method is BasicPNN))
            {
                throw new EncogError(
                    "PNN training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new TrainBasicPNN((BasicPNN) method, training);
        }
    }
}