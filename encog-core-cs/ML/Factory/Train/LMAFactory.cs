using System;
using System.Collections.Generic;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Lma;
using Encog.Util;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// This class is a factory to create the LMA training method.
    /// </summary>
    ///
    public class LMAFactory
    {
        /// <summary>
        /// Create a LMA trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public MLTrain Create(MLMethod method,
                              MLDataSet training, String argsStr)
        {
            if (!(method is BasicNetwork))
            {
                throw new EncogError(
                    "LMA training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);
            bool useReg = holder.GetBoolean(
                MLTrainFactory.PROPERTY_BAYESIAN_REGULARIZATION, false, false);

            var result = new LevenbergMarquardtTraining(
                (BasicNetwork) method, training);
            result.UseBayesianRegularization = useReg;
            return result;
        }
    }
}