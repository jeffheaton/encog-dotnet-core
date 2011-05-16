using System;
using System.Collections.Generic;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.Train;
using Encog.Neural.Flat.Train.Prop;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// A factory that creates RPROP trainers.
    /// </summary>
    ///
    public class RPROPFactory
    {
        /// <summary>
        /// Create a RPROP trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public MLTrain Create(MLMethod method,
                              MLDataSet training, String argsStr)
        {
            if (!(method is ContainsFlat))
            {
                throw new EncogError(
                    "RPROP training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);
            double initialUpdate = holder.GetDouble(
                MLTrainFactory.PROPERTY_INITIAL_UPDATE, false,
                RPROPConst.DEFAULT_INITIAL_UPDATE);
            double maxStep = holder.GetDouble(
                MLTrainFactory.PROPERTY_MAX_STEP, false,
                RPROPConst.DEFAULT_MAX_STEP);

            return new ResilientPropagation((ContainsFlat) method, training,
                                            initialUpdate, maxStep);
        }
    }
}