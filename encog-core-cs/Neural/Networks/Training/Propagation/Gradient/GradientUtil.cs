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
using Encog.Neural.Networks.Layers;
using Encog.Util;
using Encog.Neural.Data;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Synapse;

namespace Encog.Neural.Networks.Training.Propagation.Gradient
{
    /// <summary>
    /// Single threaded class that actually calculates the gradients. This is used by
    /// the individual gradient worker classes.
    /// </summary>
    public class GradientUtil
    {
        /// <summary>
        /// The network we are calculating for.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The error deltas for each layer.
        /// </summary>
        private IDictionary<ILayer, Object> layerDeltas = new Dictionary<ILayer, Object>();

        /// <summary>
        /// The gradients.
        /// </summary>
        private double[] errors;

        /// <summary>
        /// The weights.
        /// </summary>
        private double[] weights;

        /// <summary>
        /// The output from the last iteration of the network.
        /// </summary>
        private NeuralOutputHolder holder;

        /// <summary>
        /// The RMS error.
        /// </summary>
        private ErrorCalculation error = new ErrorCalculation();

        /// <summary>
        /// The number of training patterns.
        /// </summary>
        private int count;

        /// <summary>
        /// Construct the gradient utility. 
        /// </summary>
        /// <param name="network">The network to calculate gradients for.</param>
        public GradientUtil(BasicNetwork network)
        {
            this.network = network;
            int size = network.Structure.CalculateSize();
            this.errors = new double[size];
            this.holder = new NeuralOutputHolder();
        }
    
        /// <summary>
        /// Calculate the gradents between the input and ideal data.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="ideal">The desired output data.</param>
        public void Calculate(INeuralData input, INeuralData ideal)
        {
            ClearDeltas();
            this.count++;
            this.holder = new NeuralOutputHolder();
            ILayer output = this.network.GetLayer(BasicNetwork.TAG_OUTPUT);
            INeuralData actual = this.network.Compute(input, this.holder);

            this.error.UpdateError(actual.Data, ideal.Data);

            double[] deltas = GetLayerDeltas(output);
            double[] idealData = ideal.Data;
            double[] actualData = actual.Data;

            if (output.ActivationFunction.HasDerivative)
            {
                for (int i = 0; i < deltas.Length; i++)
                {
                    deltas[i] = actual.Data[i];
                }

                // take the derivative of these outputs
                output.ActivationFunction.DerivativeFunction(deltas);

                // multiply by the difference between the actual and idea
                for (int i = 0; i < output.NeuronCount; i++)
                {
                    deltas[i] = deltas[i] * (idealData[i] - actualData[i]);
                }
            }
            else
            {
                for (int i = 0; i < output.NeuronCount; i++)
                {
                    deltas[i] = (idealData[i] - actualData[i]);
                }
            }

            int index = 0;
            foreach (ILayer layer in this.network.Structure.Layers)
            {
                double[] layerDeltas = GetLayerDeltas(layer);

                if (layer.HasThreshold)
                {
                    foreach (double layerDelta in layerDeltas)
                    {
                        this.errors[index++] += layerDelta;
                    }
                }

                foreach (ISynapse synapse in this.network.Structure
                        .GetPreviousSynapses(layer))
                {
                    if (synapse.WeightMatrix != null)
                    {
                        index = Calculate(synapse, index);
                    }
                }
            }
        }
      
        /// <summary>
        /// Calculate for an entire training set. 
        /// </summary>
        /// <param name="training">The training set to use.</param>
        /// <param name="weights">The weights to use.</param>
        public void Calculate(INeuralDataSet training, double[] weights)
        {
            Reset(weights);
            foreach (INeuralDataPair pair in training)
            {
                Calculate(pair.Input, pair.Ideal);
            }
        }

        /// <summary>
        /// Calculate for an individual synapse.  
        /// </summary>
        /// <param name="synapse">The synapse to calculate for.</param>
        /// <param name="index">The current index in the weight array.</param>
        /// <returns>The new index value.</returns>
        private int Calculate(ISynapse synapse, int index)
        {

            double[] toDeltas = GetLayerDeltas(synapse.ToLayer);
            double[] fromDeltas = GetLayerDeltas(synapse.FromLayer);

            INeuralData actual = this.holder.Result[synapse];
            double[] actualData = actual.Data;

            for (int x = 0; x < synapse.ToNeuronCount; x++)
            {
                for (int y = 0; y < synapse.FromNeuronCount; y++)
                {
                    double value = actualData[y] * toDeltas[x];
                    this.errors[index] += value;
                    fromDeltas[y] += this.weights[index] * toDeltas[x];
                    index++;
                }
            }

            double[] temp = new double[fromDeltas.Length];

            for (int i = 0; i < fromDeltas.Length; i++)
            {
                temp[i] = actualData[i];
            }

            // get an activation function to use
            synapse.FromLayer.ActivationFunction.DerivativeFunction(temp);

            for (int i = 0; i < temp.Length; i++)
            {
                fromDeltas[i] *= temp[i];
            }

            return index;
        }

        /// <summary>
        /// Clear any deltas.
        /// </summary>
        private void ClearDeltas()
        {
            foreach (Object obj in this.layerDeltas.Values)
            {
                double[] d = (double[])obj;
                for (int i = 0; i < d.Length; i++)
                {
                    d[i] = 0;
                }
            }
        }

        /// <summary>
        /// The training set count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// The currenht error.
        /// </summary>
        public double Error
        {
            get
            {
                return this.error.CalculateRMS();
            }
        }

        /// <summary>
        /// The gradients.
        /// </summary>
        public double[] Errors
        {
            get
            {
                return this.errors;
            }
        }

        /// <summary>
        /// Get the deltas for a layer.  The deltas are the difference between actual and ideal.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns>The deltas as an array.</returns>
        private double[] GetLayerDeltas(ILayer layer)
        {
            if (this.layerDeltas.ContainsKey(layer))
            {
                return (double[])this.layerDeltas[layer];
            }

            double[] result = new double[layer.NeuronCount];
            this.layerDeltas[layer] = result;
            return result;
        }

        /// <summary>
        /// Reset for an iteration. 
        /// </summary>
        /// <param name="weights">The weights.</param>
        public void Reset(double[] weights)
        {
            this.error.Reset();
            this.weights = weights;
            this.layerDeltas.Clear();
            this.count = 0;

            for (int i = 0; i < this.errors.Length; i++)
            {
                this.errors[i] = 0;
            }
        }

    }
}
