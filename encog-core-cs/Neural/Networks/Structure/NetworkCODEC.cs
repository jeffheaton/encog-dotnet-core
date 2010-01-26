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
using Encog.Neural.Networks.Synapse;

#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Structure
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

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(NetworkCODEC));
#endif

        /// <summary>
        /// Use an array to populate the memory of the neural network.
        /// </summary>
        /// <param name="array">An array of doubles.</param>
        /// <param name="network">The network to encode.</param>
        public static void ArrayToNetwork(double[] array,
                 BasicNetwork network)
        {

            int index = 0;

            foreach (ILayer layer in network.Structure.Layers)
            {
                if (layer.HasThreshold)
                {
                    // process layer thresholds
                    for (int i = 0; i < layer.NeuronCount; i++)
                    {
                        layer.Threshold[i] = array[index++];
                    }
                }

                // process synapses
                foreach (ISynapse synapse in network.Structure
                        .GetPreviousSynapses(layer))
                {
                    if (synapse.WeightMatrix != null)
                    {
                        // process each weight matrix
                        for (int x = 0; x < synapse.ToNeuronCount; x++)
                        {
                            for (int y = 0; y < synapse.FromNeuronCount; y++)
                            {
                                synapse.WeightMatrix[y, x] = array[index++];
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determine if the two neural networks are equal. 
        /// </summary>
        /// <param name="network1">The first network.</param>
        /// <param name="network2">The second network.</param>
        /// <param name="precision">How many decimal places to check.</param>
        /// <returns>True if the two networks are equal.</returns>
        public static bool Equals(BasicNetwork network1,
                 BasicNetwork network2, int precision)
        {
            double[] array1 = NetworkCODEC.NetworkToArray(network1);
            double[] array2 = NetworkCODEC.NetworkToArray(network2);

            if (array1.Length != array2.Length)
            {
                return false;
            }

            double test = Math.Pow(10.0, precision);
            if (double.IsInfinity(test) || (test > long.MaxValue))
            {
                String str = "Precision of " + precision
                       + " decimal places is not supported.";
#if logging
                if (NetworkCODEC.LOGGER.IsErrorEnabled)
                {
                    NetworkCODEC.LOGGER.Error(str);
                }
#endif
                throw new NeuralNetworkError(str);
            }

            foreach (double element in array1)
            {
                long l1 = (long)(element * test);
                long l2 = (long)(element * test);
                if (l1 != l2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Convert to an array. This is used with some training algorithms that
        /// require that the "memory" of the neuron(the weight and threshold values)
        /// be expressed as a linear array. 
        /// </summary>
        /// <param name="network">The network to encode.</param>
        /// <returns>The memory of the neuron.</returns>
        public static double[] NetworkToArray(BasicNetwork network)
        {
            int size = network.Structure.CalculateSize();

            // allocate an array to hold
            double[] result = new double[size];

            int index = 0;

            foreach (ILayer layer in network.Structure.Layers)
            {
                // process layer thresholds
                if (layer.HasThreshold)
                {
                    for (int i = 0; i < layer.NeuronCount; i++)
                    {
                        result[index++] = layer.Threshold[i];
                    }
                }

                // process synapses
                foreach (ISynapse synapse in network.Structure
                        .GetPreviousSynapses(layer))
                {
                    if (synapse.WeightMatrix != null)
                    {
                        // process each weight matrix
                        for (int x = 0; x < synapse.ToNeuronCount; x++)
                        {
                            for (int y = 0; y < synapse.FromNeuronCount; y++)
                            {
                                result[index++] = synapse.WeightMatrix[y, x];
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        private NetworkCODEC()
        {

        }
    }
}
