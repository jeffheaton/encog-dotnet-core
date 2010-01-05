// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Anneal;
using Encog.Neural.NeuralData;
using Encog.Util.MathUtil;
#if logging
using log4net;
using Encog.Neural.Networks.Structure;
#endif
namespace Encog.Neural.Networks.Training.Anneal
{
    /// <summary>
    /// This class implements a simulated annealing training algorithm for 
    /// neural networks. It is based on the generic SimulatedAnnealing class.
    /// It is used in the same manner as any other training class that implements the
    /// Train interface.  This class is abstract, to create your own version
    /// of simulated annealing, you must provide an implementation of the
    /// determineError method.  If you want to train with a training set, use
    /// the NeuralTrainingSetSimulatedAnnealing class.
    /// </summary>
    public abstract class NeuralSimulatedAnnealing : BasicTraining
    {

        /// <summary>
        /// The cutoff for random data.
        /// </summary>
        public const double CUT = 0.5;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(NeuralSimulatedAnnealing));
#endif

        /// <summary>
        /// The neural network that is to be trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// This class actually performs the training.
        /// </summary>
        private NeuralSimulatedAnnealingHelper anneal;

        /// <summary>
        /// Used to calculate the score.
        /// </summary>
        private ICalculateScore calculateScore;

        /// <summary>
        /// Construct a simulated annleaing trainer for a feedforward neural network. 
        /// </summary>
        /// <param name="network">The neural network to be trained.</param>
        /// <param name="calculateScore">Used to calculate the score for a neural network.</param>
        /// <param name="startTemp">The starting temperature.</param>
        /// <param name="stopTemp">The ending temperature.</param>
        /// <param name="cycles">The number of cycles in a training iteration.</param>
        public NeuralSimulatedAnnealing(BasicNetwork network,
                ICalculateScore calculateScore,
                double startTemp,
                double stopTemp,
                int cycles)
        {
            this.network = network;
            this.calculateScore = calculateScore;
            this.anneal = new NeuralSimulatedAnnealingHelper(this);
            this.anneal.Temperature = startTemp;
            this.anneal.StartTemperature = startTemp;
            this.anneal.StopTemperature = stopTemp;
            this.anneal.Cycles = cycles;
        }

        /// <summary>
        /// Get the best network from the training.
        /// </summary>
        public override BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// Perform one iteration of simulated annealing.
        /// </summary>
        public override void Iteration()
        {
            if (NeuralSimulatedAnnealing.logger.IsInfoEnabled)
            {
                NeuralSimulatedAnnealing.logger.Info("Performing Simulated Annealing iteration.");
            }
            PreIteration();
            this.anneal.Iteration();
            this.Error = this.anneal.PerformScoreCalculation();
            PostIteration();
        }

        /// <summary>
        /// Get the network as an array of doubles. 
        /// </summary>
        /// <returns>The network as an array of doubles.</returns>
        public double[] GetArray()
        {
            return NetworkCODEC
                    .NetworkToArray(network);
        }

        /// <summary>
        /// Returns a copy of the annealing array.
        /// </summary>
        /// <returns>A copy of the annealing array.</returns>
        public double[] GetArrayCopy()
        {
            return GetArray();
        }

        /// <summary>
        /// Convert an array of doubles to the current best network. 
        /// </summary>
        /// <param name="array">An array.</param>
        public void PutArray(double[] array)
        {
            NetworkCODEC.ArrayToNetwork(array, network);
        }

        /// <summary>
        /// Randomize the weights and thresholds. This function does most of the
        /// work of the class. Each call to this class will randomize the data
        /// according to the current temperature. The higher the temperature the
        /// more randomness. 
        /// </summary>
        public void Randomize()
        {
            double[] array = NetworkCODEC
                    .NetworkToArray(network);

            for (int i = 0; i < array.Length; i++)
            {
                double add = NeuralSimulatedAnnealing.CUT - ThreadSafeRandom.NextDouble();
                add /= this.anneal.StartTemperature;
                add *= this.anneal.Temperature;
                array[i] = array[i] + add;
            }

            NetworkCODEC.ArrayToNetwork(array, network);
        }

        /// <summary>
        /// The object used to calculate the score.
        /// </summary>
        public ICalculateScore CalculateScore
        {
            get
            {
                return calculateScore;
            }
        }

    }
}
