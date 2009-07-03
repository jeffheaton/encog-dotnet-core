using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Resilient;

namespace Encog.Util.Banchmark
{
    /// <summary>
    /// Used to evaluate the training time for a network.
    /// </summary>
    public class Evaluate
    {
        /// <summary>
        /// Mili-seconds in a second.
        /// </summary>
        public const double MILIS = 1000;

        /// <summary>
        /// Miliseconds in a tick.
        /// </summary>
        public const double TICKS = 10000;

        /// <summary>
        /// How many times to try.
        /// </summary>
        public const int TRYS = 10;

        /// <summary>
        /// Private constructor.
        /// </summary>
        private Evaluate()
        {
        }

        /// <summary>
        /// Evaluate how long it takes to calculate the error for the network.  This 
        /// causes each of the training pairs to be run through the network.  The 
        /// network is evaluated 10 times and the lowest time is reported.
        /// </summary>
        /// <param name="network">The network to evaluate with.</param>
        /// <param name="training">The training data to use.</param>
        /// <returns>The lowest number of seconds that each of the ten attempts took.</returns>
        public static double EvaluateNetwork(BasicNetwork network,
                 INeuralDataSet training)
        {
            // train the neural network
            long result = long.MaxValue;

            for (int i = 1; i < TRYS; i++)
            {
                long start = DateTime.Now.Ticks;
                network.CalculateError(training);
                long time = DateTime.Now.Ticks - start;
                if (time < result)
                {
                    result = time;
                }
            }

            return result / MILIS / TICKS;
        }

        /// <summary>
        /// Evaluate how long it takes to calculate the error for the network.  This 
        /// causes each of the training pairs to be run through the network.  The 
        /// network is evaluated 10 times and the lowest time is reported.
        /// </summary>
        /// <param name="network">The network to evaluate with.</param>
        /// <param name="training">The training data to use.</param>
        /// <returns>The lowest number of seconds that each of the ten attempts took.</returns>
        public static double EvaluateTrain(BasicNetwork network,
                 INeuralDataSet training)
        {
            // train the neural network
            ITrain train = new ResilientPropagation(network, training);
            long result = long.MaxValue;

            for (int i = 1; i < TRYS; i++)
            {
                long start = DateTime.Now.Ticks;
                train.Iteration();
                long time = DateTime.Now.Ticks - start;
                if (time < result)
                {
                    result = time;
                }
            }
            return result / MILIS / TICKS;
        }
    }

}
