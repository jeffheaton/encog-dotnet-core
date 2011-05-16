using System;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation.SCG;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// A factory used to create SCG trainers. 
    /// </summary>
    ///
    public class SCGFactory
    {
        /// <summary>
        /// Create a SCG trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public MLTrain Create(MLMethod method,
                              MLDataSet training, String args)
        {
            if (!(method is BasicNetwork))
            {
                throw new EncogError(
                    "SCG training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new ScaledConjugateGradient((BasicNetwork) method, training);
        }
    }
}