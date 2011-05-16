using System;
using System.Collections.Generic;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.SVM;
using Encog.ML.SVM.Training;
using Encog.ML.Train;
using Encog.Util;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// A factory to create SVM trainers.
    /// </summary>
    ///
    public class SVMFactory
    {
        /// <summary>
        /// Create a SVM trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public MLTrain Create(MLMethod method,
                              MLDataSet training, String argsStr)
        {
            if (!(method is SupportVectorMachine))
            {
                throw new EncogError(
                    "SVM Train training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            double defaultGamma = 1.0d/((SupportVectorMachine) method).InputCount;
            double defaultC = 1.0d;

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);
            double gamma = holder.GetDouble(MLTrainFactory.PROPERTY_GAMMA,
                                            false, defaultGamma);
            double c = holder.GetDouble(MLTrainFactory.PROPERTY_C, false,
                                        defaultC);

            var result = new SVMTrain((SupportVectorMachine) method, training);
            result.Gamma = gamma;
            result.C = c;
            return result;
        }
    }
}