using System;
using System.Collections.Generic;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Util;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// A factory to create simulated annealing trainers.
    /// </summary>
    ///
    public class AnnealFactory
    {
        /// <summary>
        /// Create an annealing trainer.
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
                throw new TrainingError(
                    "Invalid method type, requires BasicNetwork");
            }

            ICalculateScore score = new TrainingSetScore(training);

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);
            double startTemp = holder.GetDouble(
                MLTrainFactory.PROPERTY_TEMPERATURE_START, false, 10);
            double stopTemp = holder.GetDouble(
                MLTrainFactory.PROPERTY_TEMPERATURE_STOP, false, 2);

            int cycles = holder.GetInt(MLTrainFactory.CYCLES, false, 100);

            MLTrain train = new NeuralSimulatedAnnealing(
                (BasicNetwork) method, score, startTemp, stopTemp, cycles);

            return train;
        }
    }
}