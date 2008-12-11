// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
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
using Encog.Matrix;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Training.Anneal
{
    /// <summary>
    /// NeuralSimulatedAnnealing: This class implements a simulated annealing
    /// training algorithm for feed forward neural networks. It is based on the
    /// generic SimulatedAnnealing class. It is used in the same manner as any other
    /// training class that implements the Train interface.
    /// </summary>
    public class NeuralSimulatedAnnealing : SimulatedAnnealing<double>, ITrain
    {
        /// <summary>
        /// Get the best network from the training.
        /// </summary>
        public BasicNetwork TrainedNetwork
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// The neural network that is to be trained.
        /// </summary>
        protected BasicNetwork network;

        /// <summary>
        /// The training set to use.
        /// </summary>
        protected INeuralDataSet training;

        /// <summary>
        /// Construct a simulated annleaing trainer for a feedforward neural network.
        /// </summary>
        /// <param name="network">The neural network to be trained.</param>
        /// <param name="training">The training set.</param>
        /// <param name="startTemp">The starting temperature.</param>
        /// <param name="stopTemp">The ending temperature.</param>
        /// <param name="cycles">The number of cycles in a training iteration.</param>
        public NeuralSimulatedAnnealing(BasicNetwork network,
                 INeuralDataSet training,
                 double startTemp, double stopTemp, int cycles)
        {
            this.network = network;
            this.training = training;
            this.temperature = startTemp;
            this.StartTemperature = startTemp;
            this.StopTemperature = stopTemp;
            this.Cycles = cycles;
        }


        /// <summary>
        /// Determine the error of the current weights and thresholds.
        /// </summary>
        /// <returns>The error.</returns>
        override public double DetermineError()
        {
            return this.network.CalculateError(this.training);
        }

        /// <summary>
        /// Get the network as an array of doubles.
        /// </summary>
        /// <returns>The network as an array of doubles.</returns>
        override public double[] GetArray()
        {
            return MatrixCODEC.NetworkToArray(this.network);
        }

        /// <summary>
        /// Convert an array of doubles to the current best network.
        /// </summary>
        /// <param name="array">The array that contains the neural network.</param>
        override public void PutArray(double[] array)
        {
            MatrixCODEC.ArrayToNetwork(array, this.network);
        }

        /// <summary>
        /// Randomize the weights and thresholds. This function does most of the work
        /// of the class. Each call to this class will randomize the data according
        /// to the current temperature. The higher the temperature the more
        /// randomness.
        /// </summary>
        override public void Randomize()
        {
            Random rand = new Random();
            double[] array = MatrixCODEC.NetworkToArray(this.network);

            for (int i = 0; i < array.Length; i++)
            {
                double add = 0.5 - (rand.NextDouble());
                add /= this.StartTemperature;
                add *= this.temperature;
                array[i] = array[i] + add;
            }

            MatrixCODEC.ArrayToNetwork(array, this.network);
        }

        /// <summary>
        /// Get the network as an array of doubles.
        /// </summary>
        /// <returns>The network as an array of doubles.</returns>
        override public double[] GetArrayCopy()
        {
            return this.GetArray();
        }
    }
}
