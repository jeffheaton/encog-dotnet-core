using System;
using System.Collections.Generic;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation.Quick;
using Encog.Util;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// A factory for quick propagation training.
    /// </summary>
    public class QuickPropFactory
    {        
        /// <summary>
        /// Create a quick propagation trainer.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                               IMLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 2.0);

            return new QuickPropagation((BasicNetwork) method, training, learningRate);
        }
    }
}