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
using Encog.Persist;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;
using Encog.Neural.Networks.Structure;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// Interface that defines a neural network.
    /// </summary>
    public interface INetwork : IEncogPersistedObject
    {
        /// <summary>
        /// Add a layer to the neural network. The first layer added is the input
        /// layer, the last layer added is the output layer. This layer is added with
        /// a weighted synapse.
        /// </summary>
        /// <param name="layer">The layer to be added.</param>
        void AddLayer(ILayer layer);


        /// <summary>
        /// Add a layer to the neural network. If there are no layers added this
        /// layer will become the input layer. This function automatically updates
        /// both the input and output layer references.
        /// </summary>
        /// <param name="layer">The layer to be added to the network.</param>
        /// <param name="type">What sort of synapse should connect this layer to the last.</param>
        void AddLayer(ILayer layer, SynapseType type);

        /// <summary>
        /// Calculate the error for this neural network. The error is calculated
        /// using root-mean-square(RMS).
        /// </summary>
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        double CalculateError(INeuralDataSet data);

        /// <summary>
        /// Calculate the total number of neurons in the network across all layers.
        /// </summary>
        /// <returns>The neuron count.</returns>
        int CalculateNeuronCount();

        /// <summary>
        /// Compute the output for a given input to the neural network.
        /// </summary>
        /// <param name="input">The input to the neural network.</param>
        /// <returns>The output from the neural network.</returns>
        INeuralData Compute(INeuralData input);

        /// <summary>
        /// Compute the output for a given input to the neural network. This method
        /// provides a parameter to specify an output holder to use. This holder
        /// allows propagation training to track the output from each layer. If you
        /// do not need this holder pass null, or use the other compare method.
        /// </summary>
        /// <param name="input">The input provide to the neural network.</param>
        /// <param name="useHolder">Allows a holder to be specified, this allows propagation
        /// training to check the output of each layer.</param>
        /// <returns>The results from the output neurons.</returns>
        INeuralData Compute(INeuralData input,
                 NeuralOutputHolder useHolder);

        /// <summary>
        /// Compare the two neural networks. For them to be equal they must be of the
        /// same structure, and have the same matrix values.
        /// </summary>
        /// <param name="other">The other neural network.</param>
        /// <returns>True if the two networks are equal.</returns>
        bool Equals(BasicNetwork other);

        /// <summary>
        /// Determine if this neural network is equal to another. Equal neural
        /// networks have the same weight matrix and threshold values, within a
        /// specified precision.
        /// </summary>
        /// <param name="other">The other neural network.</param>
        /// <param name="precision">The number of decimal places to compare to.</param>
        /// <returns>True if the two neural networks are equal.</returns>
        bool Equals(BasicNetwork other, int precision);

        /// <summary>
        /// Get the structure of the neural network. The structure allows you
        /// to quickly obtain synapses and layers without traversing the
        /// network.
        /// </summary>
        NeuralStructure Structure
        {
            get;
        }

        /// <summary>
        /// The size of the matrix.
        /// </summary>
        int WeightMatrixSize
        {
            get;
        }

        /// <summary>
        /// Generate a hash code.
        /// </summary>
        /// <returns></returns>
        int GetHashCode();

        /// <summary>
        /// Reset the weight matrix and the thresholds.
        /// </summary>
        void Reset();

        /// <summary>
        /// Convert this object to a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        String ToString();

        /// <summary>
        /// Determine the winner for the specified input. This is the number of the
        /// winning neuron.
        /// </summary>
        /// <param name="input">The input patter to present to the neural network.</param>
        /// <returns>The winning neuron.</returns>
        int Winner(INeuralData input);
    }
}
