// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks.Structure;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// Implementation of <i>Nguyen-Widrow</i> weight initialization. This is the
    /// default weight initialization used by Encog, as it generally provides the
    /// most trainable neural network.
    /// 
    /// 
    /// author(from Java Version) StŽphan Corriveau
    /// </summary>
    public class NguyenWidrowRandomizer : RangeRandomizer
    {
        /// <summary>
        /// Construct a Nguyen-Widrow randomizer.
        /// </summary>
        /// <param name="min">The min of the range.</param>
        /// <param name="max">The max of the range.</param>
        public NguyenWidrowRandomizer(double min, double max)
            : base(min, max)
        {

        }



        /// <summary>
        /// The Nguyen-Widrow initialization algorithm is the following :
        /// 
        /// 1. Initialize all weight of hidden layers with (ranged) random values
        /// 2. For each hidden layer
        /// 2.1 calculate beta value, 0.7 * Nth(#neurons of input layer) root of
        /// #neurons of current layer 
        /// 2.2 for each synapse
        /// 2.1.1 for each weight 
        /// 2.1.2 Adjust weight by dividing by norm of weight for neuron and
        /// multiplying by beta value
        /// </summary>
        /// <param name="network">The network to randomize.</param>
        public override void Randomize(BasicNetwork network)
        {
            base.Randomize(network);
            int neuronCount = 0;

            foreach (ILayer layer in network.Structure.Layers)
            {
                neuronCount += layer.NeuronCount;
            }

            ILayer inputLayer = network.GetLayer(BasicNetwork.TAG_INPUT);
            ILayer outputLayer = network.GetLayer(BasicNetwork.TAG_OUTPUT);

            if (inputLayer == null)
                throw new EncogError("Must have an input layer for Nguyen-Widrow.");

            if (outputLayer == null)
                throw new EncogError("Must have an output layer for Nguyen-Widrow.");

            int hiddenNeurons = neuronCount - inputLayer.NeuronCount
                    - outputLayer.NeuronCount;

            if (hiddenNeurons < 1)
                throw new EncogError("Must have hidden neurons for Nguyen-Widrow.");

            double beta = 0.7 * Math.Pow(hiddenNeurons, 1.0 / inputLayer
                    .NeuronCount);

            foreach (ISynapse synapse in network.Structure.Synapses)
            {
                Randomize(beta, synapse);
            }

            network.Structure.FlatUpdate = FlatUpdateNeeded.Flatten;
            network.Structure.FlattenWeights();
        }

        /// <summary>
        /// Randomize the specified synapse.
        /// </summary>
        /// <param name="beta">The beta value.</param>
        /// <param name="synapse">The synapse to modify.</param>
        private void Randomize(double beta, ISynapse synapse)
        {
            if (synapse.WeightMatrix == null)
                return;

            for (int j = 0; j < synapse.ToNeuronCount; j++)
            {
                double norm = 0.0;

                // Calculate the Euclidean Norm for the weights
                for (int k = 0; k < synapse.FromNeuronCount; k++)
                {
                    double v = synapse.WeightMatrix[k, j];
                    norm += v * v;
                }

                if (synapse.ToLayer.HasBias)
                {
                    double value = synapse.ToLayer.BiasWeights[j];
                    norm += value * value;
                }


                norm = Math.Sqrt(norm);

                // Rescale the weights using beta and the norm
                for (int k = 0; k < synapse.FromNeuronCount; k++)
                {
                    double value = synapse.WeightMatrix[k, j];
                    synapse.WeightMatrix[k, j] = beta * value / norm;
                }

                if (synapse.ToLayer.HasBias)
                {
                    double value = synapse.ToLayer.BiasWeights[j];
                    synapse.ToLayer.BiasWeights[j] = beta * value / norm;
                }
            }
        }
    }
}
