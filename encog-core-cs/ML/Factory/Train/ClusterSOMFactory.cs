using System;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.SOM;
using Encog.Neural.Som.Training.Clustercopy;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// Create a trainer that uses the SOM cluster training method.
    /// </summary>
    ///
    public class ClusterSOMFactory
    {
        /// <summary>
        /// Create a cluster SOM trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public MLTrain Create(MLMethod method,
                              MLDataSet training, String argsStr)
        {
            if (!(method is SOMNetwork))
            {
                throw new EncogError(
                    "Cluster SOM training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new SOMClusterCopyTraining((SOMNetwork) method, training);
        }
    }
}