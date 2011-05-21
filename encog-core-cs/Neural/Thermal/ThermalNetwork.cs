//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Specific;
using Encog.Util;
using System;

namespace Encog.Neural.Thermal
{
    /// <summary>
    /// The thermal network forms the base class for Hopfield and Boltzmann machines.
    /// </summary>
    [Serializable]
    public abstract class ThermalNetwork : BasicML, IMLMethod,
                                           IMLAutoAssocation, IMLResettable
    {
        /// <summary>
        /// The current state of the thermal network.
        /// </summary>
        ///
        private BiPolarMLData currentState;

        /// <summary>
        /// The neuron count.
        /// </summary>
        ///
        private int neuronCount;

        /// <summary>
        /// The weights.
        /// </summary>
        ///
        private double[] weights;

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public ThermalNetwork()
        {
        }

        /// <summary>
        /// Construct the network with the specicified neuron count.
        /// </summary>
        ///
        /// <param name="neuronCount_0">The number of neurons.</param>
        public ThermalNetwork(int neuronCount_0)
        {
            neuronCount = neuronCount_0;
            weights = new double[neuronCount_0*neuronCount_0];
            currentState = new BiPolarMLData(neuronCount_0);
        }

        /// <summary>
        /// Set the neuron count.
        /// </summary>
        public int NeuronCount
        {
            get { return neuronCount; }
            set { neuronCount = value; }
        }

        /// <summary>
        /// Set the weight array.
        /// </summary>
        ///
        /// <value>The weight array.</value>
        public double[] Weights
        {
            get { return weights; }
            set { weights = value; }
        }

        /// <summary>
        /// Set the current state.
        /// </summary>
        public BiPolarMLData CurrentState
        {
            get { return currentState; }
            set
            {
                for (int i = 0; i < value.Count; i++)
                {
                    currentState[i] = value[i];
                }
            }
        }

        #region MLAutoAssocation Members

        /// <summary>
        /// from Encog.ml.MLInput
        /// </summary>
        public abstract int InputCount { get; }


        /// <summary>
        /// from Encog.ml.MLOutput
        /// </summary>
        ///
        public abstract int OutputCount { get; }


        /// <summary>
        /// from Encog.ml.MLRegression
        /// </summary>
        ///
        public abstract IMLData Compute(
            IMLData input);

        #endregion

        #region MLResettable Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Reset()
        {
            Reset(0);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Reset(int seed)
        {
            CurrentState.Clear();
            EngineArray.Fill(weights, 0.0d);
        }

        #endregion

        /// <summary>
        /// Add to the specified weight.
        /// </summary>
        ///
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <param name="value_ren">The value to add.</param>
        public void AddWeight(int fromNeuron, int toNeuron,
                              double value_ren)
        {
            int index = (toNeuron*neuronCount) + fromNeuron;
            if (index >= weights.Length)
            {
                throw new NeuralNetworkError("Out of range: fromNeuron:"
                                             + fromNeuron + ", toNeuron: " + toNeuron);
            }
            weights[index] += value_ren;
        }


        /// <returns>Calculate the current energy for the network. The network will
        /// seek to lower this value.</returns>
        public double CalculateEnergy()
        {
            double tempE = 0;
            int neuronCount_0 = NeuronCount;

            for (int i = 0; i < neuronCount_0; i++)
            {
                for (int j = 0; j < neuronCount_0; j++)
                {
                    if (i != j)
                    {
                        tempE += GetWeight(i, j)*currentState[i]
                                 *currentState[j];
                    }
                }
            }
            return -1*tempE/2;
        }

        /// <summary>
        /// Clear any connection weights.
        /// </summary>
        ///
        public void Clear()
        {
            EngineArray.Fill(weights, 0);
        }


        /// <summary>
        /// Get a weight.
        /// </summary>
        ///
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <returns>The weight.</returns>
        public double GetWeight(int fromNeuron, int toNeuron)
        {
            int index = (toNeuron*neuronCount) + fromNeuron;
            return weights[index];
        }


        /// <summary>
        /// Init the network.
        /// </summary>
        ///
        /// <param name="neuronCount_0">The neuron count.</param>
        /// <param name="weights_1">The weights.</param>
        /// <param name="output">The toutpu</param>
        public void Init(int neuronCount_0, double[] weights_1,
                         double[] output)
        {
            if (neuronCount_0 != output.Length)
            {
                throw new NeuralNetworkError("Neuron count(" + neuronCount_0
                                             + ") must match output count(" + output.Length + ").");
            }

            if ((neuronCount_0*neuronCount_0) != weights_1.Length)
            {
                throw new NeuralNetworkError("Weight count(" + weights_1.Length
                                             + ") must be the square of the neuron count(" + neuronCount_0
                                             + ").");
            }

            neuronCount = neuronCount_0;
            weights = weights_1;
            currentState = new BiPolarMLData(neuronCount_0);
            currentState.Data = output;
        }

        /// <summary>
        /// Set the current state.
        /// </summary>
        /// <param name="s">The new current state.</param>
        public void SetCurrentState(double[] s)
        {
            currentState = new BiPolarMLData(s.Length);
            EngineArray.ArrayCopy(s, currentState.Data);
        }

        /// <summary>
        /// Set the weight.
        /// </summary>
        ///
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <param name="value_ren">The value.</param>
        public void SetWeight(int fromNeuron, int toNeuron,
                              double value_ren)
        {
            int index = (toNeuron*neuronCount) + fromNeuron;
            weights[index] = value_ren;
        }
    }
}
