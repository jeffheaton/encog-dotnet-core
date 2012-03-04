using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil.Randomize;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.PSO;
using Encog.Util;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// Factory to create PSO trainers.
    /// </summary>
    public class PSOFactory
    {
        /// <summary>
        /// Create a PSO trainer.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                IMLDataSet training, String argsStr)
        {

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            ParamsHolder holder = new ParamsHolder(args);

            int particles = holder.GetInt(
                    MLTrainFactory.PropertyParticles, false, 20);

            ICalculateScore score = new TrainingSetScore(training);
            IRandomizer randomizer = new NguyenWidrowRandomizer();

            IMLTrain train = new NeuralPSO((BasicNetwork)method, randomizer, score, particles);

            return train;
        }
    }
}
