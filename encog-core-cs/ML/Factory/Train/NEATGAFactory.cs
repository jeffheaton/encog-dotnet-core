using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Train;
using Encog.ML.Data;
using Encog.Neural.Networks.Training;
using Encog.Neural.NEAT;
using Encog.ML.EA.Train;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// A factory to create genetic algorithm trainers.
    /// </summary>
    public class NEATGAFactory
    {
        /// <summary>
        /// Create an NEAT GA trainer.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                IMLDataSet training, String argsStr)
        {

            ICalculateScore score = new TrainingSetScore(training);
            TrainEA train = NEATUtil.ConstructNEATTrainer((NEATPopulation)method, score);

            return train;
        }
    }
}
