using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.NM;
using Encog.Util;

namespace Encog.ML.Factory.Train
{
    public class NelderMeadFactory
    {
        /// <summary>
        /// Create a Nelder Mead trainer.
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

            //final double learningRate = holder.getDouble(
            //		MLTrainFactory.PROPERTY_LEARNING_RATE, false, 0.1);

            return new NelderMeadTraining((BasicNetwork)method, training);
        }
    }
}
