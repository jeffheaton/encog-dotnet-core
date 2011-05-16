using System;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.RBF;
using Encog.Neural.Rbf.Training;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// This factory is used to create a SVD trainer for an RBF network.
    /// </summary>
    ///
    public class RBFSVDFactory
    {
        /// <summary>
        /// Create a RBF-SVD trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public MLTrain Create(MLMethod method,
                              MLDataSet training, String args)
        {
            if (!(method is RBFNetwork))
            {
                throw new EncogError(
                    "RBF-SVD training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new SVDTraining((RBFNetwork) method, training);
        }
    }
}