// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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
#endif
namespace Encog.Neural.Networks.Training.Anneal
{
    /// <summary>
    /// This class implements a simulated annealing training algorithm for feed
    /// forward neural networks. It is based on the generic SimulatedAnnealing class.
    /// It is used in the same manner as any other training class that implements the
    /// Train interface.
    /// </summary>
    public class NeuralSimulatedAnnealing : BasicTraining
    {
        /// <summary>
        /// Simple inner class used by the neural simulated annealing.  This
        /// class is a subclass of the basic SimulatedAnnealing class.  The
        /// It is used by the actual NeuralSimulatedAnnealing class, which
        /// subclasses BasicTraining.  This class is mostly necessary due
        /// to the fact that NeuralSimulatedAnnealing can't subclass BOTH
        /// SimulatedAnnealing and Train, because multiple inheritance is
        /// not supported.
        /// </summary>
        class SimulatedAnnealingHelper : SimulatedAnnealing<Double>
        {
            private NeuralSimulatedAnnealing owner;

            public SimulatedAnnealingHelper(NeuralSimulatedAnnealing owner)
            {
                this.owner = owner;
            }

            /// <summary>
            /// Determine the error of the nueral network.
            /// </summary>
            /// <returns>The neural network error.</returns>
            public override double DetermineError()
            {
                return owner.Network.CalculateError(owner.Training);
            }

            /// <summary>
            /// Get the network as an array of doubles.
            /// </summary>
            /// <returns>The network as an array of doubles.</returns>
            public override double[] GetArray()
            {
                return NetworkCODEC
                        .NetworkToArray(owner.Network);
            }

            /// <summary>
            /// A copy of the annealing array.
            /// </summary>
            /// <returns>A copy of the annealing array.</returns>
            public override double[] GetArrayCopy()
            {
                return GetArray();
            }

            /// <summary>
            /// Convert an array of doubles to the current best network.
            /// </summary>
            /// <param name="array">An array.</param>
            public override void PutArray(double[] array)
            {
                NetworkCODEC.ArrayToNetwork(array,
                        owner.network);
            }

            /// <summary>
            /// Randomize the weights and thresholds. This function does most of the
            /// work of the class. Each call to this class will randomize the data
            /// according to the current temperature. The higher the temperature the
            /// more randomness.
            /// </summary>
            public override void Randomize()
            {
                Double[] array = NetworkCODEC
                       .NetworkToArray(owner.Network);

                for (int i = 0; i < array.Length; i++)
                {
                    double add = NeuralSimulatedAnnealing.CUT - ThreadSafeRandom.NextDouble();
                    add /= this.StartTemperature;
                    add *= this.StopTemperature;
                    array[i] = array[i] + add;
                }

                NetworkCODEC.ArrayToNetwork(array,
                        owner.Network);
            }

        }

        /// <summary>
        /// The cutoff for random data.
        /// </summary>
        public const double CUT = 0.5;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(NeuralSimulatedAnnealing));
#endif

        /// <summary>
        /// The neural network that is to be trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// This class actually performs the training.
        /// </summary>
        private SimulatedAnnealingHelper anneal;

        /// <summary>
        /// Construct a simulated annleaing trainer for a feedforward neural network.
        /// </summary>
        /// <param name="network">The neural network to be trained.</param>
        /// <param name="training">The training set.</param>
        /// <param name="startTemp">The starting temperature.</param>
        /// <param name="stopTemp">The ending temperature.</param>
        /// <param name="cycles">The number of cycles in a training iteration.</param>
        public NeuralSimulatedAnnealing(BasicNetwork network,
                 INeuralDataSet training, double startTemp,
                 double stopTemp, int cycles)
        {
            this.network = network;
            this.Training = training;
            this.anneal = new SimulatedAnnealingHelper(this);
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
#if logging
            if (this.logger.IsInfoEnabled)
            {
                this.logger.Info("Performing Simulated Annealing iteration.");
            }
#endif
            PreIteration();
            this.anneal.Iteration();
            this.Error = this.anneal.DetermineError();
            PostIteration();
        }

    }
}
