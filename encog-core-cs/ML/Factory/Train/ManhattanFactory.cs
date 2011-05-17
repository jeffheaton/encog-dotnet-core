using System;
using System.Collections.Generic;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Util;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// A factory for Manhattan training.
    /// </summary>
    ///
    public class ManhattanFactory
    {
        /// <summary>
        /// Create a Manhattan trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public MLTrain Create(MLMethod method,
                              MLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PROPERTY_LEARNING_RATE, false, 0.1d);

            return new ManhattanPropagation((BasicNetwork) method, training,
                                            learningRate);
        }
    }
}