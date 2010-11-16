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
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.Engine.Util;

#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Structure
{
    /// <summary>
    /// This class will extract the "long term memory" of a neural network, that is
    /// the weights and bias values into an array. This array can be used to
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
                index = NetworkCODEC.ProcessLayer(network, layer, array, index);
            }

            network.Structure.FlatUpdate = FlatUpdateNeeded.Flatten;
        }

        /// <summary>
        /// Determine the network size.
        /// </summary>
        /// <param name="network">The network to check.</param>
        /// <returns>The size of the network.</returns>
        public static int NetworkSize(BasicNetwork network)
        {

            // see if there is already an up to date flat network
            if (network.Structure.Flat != null
                && (network.Structure.FlatUpdate == FlatUpdateNeeded.None
                || network.Structure.FlatUpdate == FlatUpdateNeeded.Unflatten))
            {
                return network.Structure.Flat.Weights.Length;
            }

            int index = 0;

            // loop over all of the layers, take the output layer first
            foreach (ILayer layer in network.Structure.Layers)
            {

                // see if the previous layer, which is the next layer that the loop will hit,
                // is either a connection to a BasicLayer or a ContextLayer.
                ISynapse synapse = network.Structure
                        .FindPreviousSynapseByLayerType(layer, typeof(BasicLayer));
                ISynapse contextSynapse = network.Structure.FindPreviousSynapseByLayerType(
                        layer, typeof(ContextLayer));

                // get a list of of the previous synapses to this layer
                IList<ISynapse> list = network.Structure.GetPreviousSynapses(layer);

                // If there is not a BasicLayer or contextLayer as the next layer, then
                // just take the first synapse of any type.
                if (synapse == null && contextSynapse == null && list.Count > 0)
                {
                    synapse = list[0];
                }

                // is there any data to record for this synapse?
                if (synapse != null && synapse.WeightMatrix != null)
                {
                    // process each weight matrix
                    for (int x = 0; x < synapse.ToNeuronCount; x++)
                    {

                        index += synapse.FromNeuronCount;


                        if (synapse.ToLayer.HasBias)
                        {
                            index++;
                        }

                        if (contextSynapse != null)
                        {
                            index += contextSynapse.FromNeuronCount;
                        }
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Process a synapse.
        /// </summary>
        /// <param name="network">The network to process.</param>
        /// <param name="layer">The layer to process.</param>
        /// <param name="array">The array to process.</param>
        /// <param name="index">The current index.</param>
        /// <returns>The index after this synapse has been read.</returns>
        private static int ProcessLayer(BasicNetwork network,
                ILayer layer, double[] array, int index)
        {
            int result = index;

            // see if the previous layer, which is the next layer that the loop will hit,
            // is either a connection to a BasicLayer or a ContextLayer.
            ISynapse synapse = network.Structure
                    .FindPreviousSynapseByLayerType(layer, typeof(BasicLayer));
            ISynapse contextSynapse = network.Structure
                    .FindPreviousSynapseByLayerType(layer, typeof(ContextLayer));

            // get a list of of the previous synapses to this layer
            IList<ISynapse> list = network.Structure.GetPreviousSynapses(layer);

            // If there is not a BasicLayer or contextLayer as the next layer, then
            // just take the first synapse of any type.
            if (synapse == null && contextSynapse == null && list.Count > 0)
            {
                synapse = list[0];
            }

            // is there any data to record for this synapse?		
            if (synapse != null && synapse.WeightMatrix != null)
            {
                // process each weight matrix
                for (int x = 0; x < synapse.ToNeuronCount; x++)
                {
                    for (int y = 0; y < synapse.FromNeuronCount; y++)
                    {
                        synapse.WeightMatrix[y, x] = array[result++];
                    }
                    if (synapse.ToLayer.HasBias)
                    {
                        synapse.ToLayer.BiasWeights[x] = array[result++];
                    }

                    if (contextSynapse != null)
                    {
                        for (int z = 0; z < contextSynapse.FromNeuronCount; z++)
                        {
                            double value = array[result++];
                            double oldValue = synapse.WeightMatrix[z, x];

                            // if this connection is limited, do not update it to anything but zero
                            if (Math.Abs(oldValue) < network.Structure
                                    .ConnectionLimit)
                            {
                                value = 0;
                            }

                            // update the actual matrix
                            contextSynapse.WeightMatrix[z, x] = value;
                        }
                    }

                }
            }

            return result;
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
        /// require that the "memory" of the neuron(the weight and bias values)
        /// be expressed as a linear array. 
        /// </summary>
        /// <param name="network">The network to encode.</param>
        /// <returns>The memory of the neuron.</returns>
        public static double[] NetworkToArray(BasicNetwork network)
        {
            int size = NetworkSize(network);

            // see if there is already an up to date flat network
            if (network.Structure.Flat != null
                && (network.Structure.FlatUpdate == FlatUpdateNeeded.None
                || network.Structure.FlatUpdate == FlatUpdateNeeded.Unflatten))
            {
                return EngineArray.ArrayCopy(network.Structure.Flat.Weights);
            }

            // allocate an array to hold
            double[] result = new double[size];

            int index = 0;

            // loop over all of the layers, take the output layer first
            foreach (ILayer layer in network.Structure.Layers)
            {

                // see if the previous layer, which is the next layer that the loop will hit,
                // is either a connection to a BasicLayer or a ContextLayer.
                ISynapse synapse = network.Structure
                        .FindPreviousSynapseByLayerType(layer, typeof(BasicLayer));
                ISynapse contextSynapse = network.Structure.FindPreviousSynapseByLayerType(
                        layer, typeof(ContextLayer));

                // get a list of of the previous synapses to this layer
                IList<ISynapse> list = network.Structure.GetPreviousSynapses(layer);

                // If there is not a BasicLayer or contextLayer as the next layer, then
                // just take the first synapse of any type.
                if (synapse == null && contextSynapse == null && list.Count > 0)
                {
                    synapse = list[0];
                }

                // is there any data to record for this synapse?
                if (synapse != null && synapse.WeightMatrix != null)
                {
                    // process each weight matrix
                    for (int x = 0; x < synapse.ToNeuronCount; x++)
                    {
                        for (int y = 0; y < synapse.FromNeuronCount; y++)
                        {
                            result[index++] = synapse.WeightMatrix[y, x];
                        }

                        if (synapse.ToLayer.HasBias)
                        {
                            result[index++] = synapse.ToLayer.BiasWeights[x];
                        }

                        if (contextSynapse != null)
                        {
                            for (int z = 0; z < contextSynapse.FromNeuronCount; z++)
                            {
                                result[index++] = contextSynapse.WeightMatrix[z, x];
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
