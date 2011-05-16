using System;
using System.Collections.Generic;
using Encog.MathUtil.Randomize;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Genetic;
using Encog.Util;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// A factory to create genetic algorithm trainers.
    /// </summary>
    ///
    public class GeneticFactory
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
            int populationSize = holder.GetInt(
                MLTrainFactory.PROPERTY_POPULATION_SIZE, false, 5000);
            double mutation = holder.GetDouble(
                MLTrainFactory.PROPERTY_MUTATION, false, 0.1d);
            double mate = holder.GetDouble(MLTrainFactory.PROPERTY_MATE,
                                           false, 0.25d);

            MLTrain train = new NeuralGeneticAlgorithm((BasicNetwork) method,
                                                       new RangeRandomizer(-1, 1), score, populationSize, mutation,
                                                       mate);

            return train;
        }
    }
}