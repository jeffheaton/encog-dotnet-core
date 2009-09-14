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
using Encog.Neural.Data;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Pattern;

#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Provides the neural logic for an BAM type network.  See BAMPattern
    /// for more information on this type of network.
    /// </summary>
    [Serializable]
    public class BAMLogic : INeuralLogic
    {
        /// <summary>
        /// The neural network.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The F1 layer.
        /// </summary>
        private ILayer f1Layer;

        /// <summary>
        /// The F2 layer.
        /// </summary>
        private ILayer f2Layer;

        /// <summary>
        /// The connection between the input and output layer.
        /// </summary>
        private ISynapse synapseF1ToF2;

        /// <summary>
        /// The connection between the output and the input layer.
        /// </summary>
        private ISynapse synapseF2ToF1;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(BAMLogic));
#endif

        /// <summary>
        /// The F1 neuron count.
        /// </summary>
        public int F1Neurons
        {
            get
            {
                return this.f1Layer.NeuronCount;
            }
        }

        /// <summary>
        /// The F2 neuron count.
        /// </summary>
        public int F2Neurons
        {
            get
            {
                return this.f2Layer.NeuronCount;
            }
        }

        /// <summary>
        /// Add a pattern to the neural network. 
        /// </summary>
        /// <param name="inputPattern">The input pattern.</param>
        /// <param name="outputPattern">The output pattern(for this input).</param>
        public void AddPattern(INeuralData inputPattern,
                 INeuralData outputPattern)
        {

            int weight;

            for (int i = 0; i < this.F1Neurons; i++)
            {
                for (int j = 0; j < this.F2Neurons; j++)
                {
                    weight = (int)(inputPattern[i] * outputPattern[j]);
                    this.synapseF1ToF2.WeightMatrix.Add(i, j, weight);
                    this.synapseF2ToF1.WeightMatrix.Add(j, i, weight);
                }
            }

        }

        /// <summary>
        /// Clear any connection weights.
        /// </summary>
        public void Clear()
        {
            this.synapseF1ToF2.WeightMatrix.Clear();
            this.synapseF2ToF1.WeightMatrix.Clear();
        }

        /// <summary>
        /// The network.
        /// </summary>
        public BasicNetwork Network
        {
            get
            {
                return network;
            }
        }

        /// <summary>
        /// Get the specified weight.
        /// </summary>
        /// <param name="synapse">The synapse to get the weight from.</param>
        /// <param name="input">The input, to obtain the size from.</param>
        /// <param name="x">The x matrix value. (could be row or column, depending on input)</param>
        /// <param name="y">The y matrix value. (could be row or column, depending on input)</param>
        /// <returns>The value from the matrix.</returns>
        private double GetWeight(ISynapse synapse, INeuralData input, int x, int y)
        {
            if (synapse.FromNeuronCount != input.Count)
                return synapse.WeightMatrix[x, y];
            else
                return synapse.WeightMatrix[y, x];
        }

        /// <summary>
        /// Propagate the layer.
        /// </summary>
        /// <param name="synapse">The synapse for this layer.</param>
        /// <param name="input">The input pattern.</param>
        /// <param name="output">The output pattern.</param>
        /// <returns>True if the network has become stable.</returns>
        private bool PropagateLayer(ISynapse synapse, INeuralData input,
                INeuralData output)
        {
            int i, j;
            int sum, outt = 0;
            bool stable;

            stable = true;

            for (i = 0; i < output.Count; i++)
            {
                sum = 0;
                for (j = 0; j < input.Count; j++)
                {
                    sum += (int)(GetWeight(synapse, input, i, j) * input[j]);
                }
                if (sum != 0)
                {
                    if (sum < 0)
                        outt = -1;
                    else
                        outt = 1;
                    if (outt != (int)output[i])
                    {
                        stable = false;
                        output[i] = outt;
                    }
                }
            }
            return stable;
        }

        /// <summary>
        /// Compute the network for the specified input.
        /// </summary>
        /// <param name="input">The input to the network.</param>
        /// <returns>The output from the network.</returns>
        public NeuralDataMapping Compute(NeuralDataMapping input)
        {
            bool stable1 = true, stable2 = true;

            do
            {

                stable1 = PropagateLayer(this.synapseF1ToF2,
                        input.From, input.To);
                stable2 = PropagateLayer(this.synapseF2ToF1, input.To,
                        input.From);


            } while (!stable1 && !stable2);
            return null;
        }

        /// <summary>
        /// Setup the network logic, read parameters from the network.
        /// NOT USED, call compute(NeuralInputData).
        /// </summary>
        /// <param name="input">The input to the layer.</param>
        /// <param name="useHolder">The holder to use.</param>
        /// <returns>The output from this layer.</returns>
        public INeuralData Compute(INeuralData input, NeuralOutputHolder useHolder)
        {
            String str = "Compute on BasicNetwork cannot be used, rather call" +
                    " the compute(NeuralData) method on the BAMLogic.";
#if logging
            if (logger.IsErrorEnabled)
            {
                logger.Error(str);
            }
#endif
            throw new NeuralNetworkError(str);
        }

        /// <summary>
        /// Setup the network logic, read parameters from the network.
        /// </summary>
        /// <param name="network">The network that this logic class belongs to.</param>
        public void Init(BasicNetwork network)
        {
            this.network = network;
            this.f1Layer = network.GetLayer(BAMPattern.TAG_F1);
            this.f2Layer = network.GetLayer(BAMPattern.TAG_F2);
            this.synapseF1ToF2 = network.Structure.FindSynapse(this.f1Layer, this.f2Layer, true);
            this.synapseF2ToF1 = network.Structure.FindSynapse(this.f2Layer, this.f1Layer, true);
        }
    }
}
