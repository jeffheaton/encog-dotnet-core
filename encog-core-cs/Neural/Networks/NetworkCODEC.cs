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
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks.Layers;
using log4net;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// This class will extract the "long term memory" of a neural network, that is
    /// the weights and threshold values into an array. This array can be used to
    /// view the neural network as a linear array of doubles. These values can then
    /// be modified and copied back into the neural network. This is very useful for
    /// simulated annealing, as well as genetic algorithms.
    /// </summary>
    public class NetworkCODEC
    {
        /// <summary>
        /// Use an array to populate the memory of the neural network.
        /// </summary>
        /// <param name="array">An array of doubles.</param>
        /// <param name="network">The network to encode.</param>
        public static void ArrayToNetwork(double[] array,
                 BasicNetwork network)
        {

            // copy all weight data
            int currentIndex = 0;
            ICollection<ISynapse> synapses = network.Structure
                   .Synapses;
            foreach (ISynapse synapse in synapses)
            {
                if (synapse.WeightMatrix != null)
                {

                    currentIndex = synapse.WeightMatrix.FromPackedArray(array,
                            currentIndex);

                }
            }

            // copy all threshold data
            foreach (ILayer layer in network.Structure.Layers)
            {
                for (int i = 0; i < layer.NeuronCount; i++)
                {
                    layer.Threshold[i] = array[currentIndex++];
                }
            }

        }

        /// <summary>
        /// Convert to an array. This is used with some training algorithms that
        /// require that the "memory" of the neuron(the weight and threshold values)
        /// be expressed as a linear array.
        /// </summary>
        /// <param name="network">The network to encode.</param>
        /// <returns>The memory of the neuron.</returns>
        public static Double[] NetworkToArray(BasicNetwork network)
        {
            int size = 0;

            // first determine size from matrixes
            foreach (ISynapse synapse in network.Structure.Synapses)
            {
                size += synapse.MatrixSize;
            }

            // determine size from threshold values
            foreach (ILayer layer in network.Structure.Layers)
            {
                size += layer.NeuronCount;
            }

            // allocate an array to hold
            double[] result = new Double[size];

            // copy all weight data
            int currentIndex = 0;
            ICollection<ISynapse> synapses = network.Structure
                   .Synapses;
            foreach (ISynapse synapse in synapses)
            {
                if (synapse.WeightMatrix != null)
                {
                    double[] temp = synapse.WeightMatrix.ToPackedArray();
                    foreach (double element in temp)
                    {
                        result[currentIndex++] = element;
                    }
                }
            }

            // copy all threshold data
            foreach (ILayer layer in network.Structure.Layers)
            {
                for (int i = 0; i < layer.NeuronCount; i++)
                {
                    result[currentIndex++] = layer.Threshold[i];
                }
            }

            return result;
        }

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(NetworkCODEC));

        /// <summary>
        /// Private constructor.
        /// </summary>
        private NetworkCODEC()
        {

        }

    }

}
