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
using Encog.Persist;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Data;
using Encog.Neural.Activation;

namespace Encog.Neural.Networks.Layers
{
    /// <summary>
    /// This interface defines all necessary methods for a neural network layer.
    /// </summary>
    public interface ILayer : ICloneable, IEncogPersistedObject
    {
        /// <summary>
        /// Add a layer to this layer.  The "next" layer being added will
        /// receive input from this layer.  You can also add a layer to
        /// itself, this will create a self-connected layer.  This method
        /// will create a weighted synapse connection between this layer
        /// and the next.
        /// </summary>
        /// <param name="next">The layer that is to be added.</param>
        void AddNext(ILayer next);

        /// <summary>
        /// Add a layer to this layer.  The "next" layer being added will
        /// receive input from this layer.  You can also add a layer to
        /// itself, this will create a self-connected layer.
        /// </summary>
        /// <param name="next">The layer that is to be added.</param>
        /// <param name="type">The type of synapse to add.</param>
        void AddNext(ILayer next, SynapseType type);

        /// <summary>
        /// This method adds a synapse to the neural network.  Usually
        /// you will want to use the addNext method rather than directly
        /// adding synapses.
        /// </summary>
        /// <param name="synapse">The synapse to add.</param>
        void AddSynapse(ISynapse synapse);

        /// <summary>
        /// Compute the output for this layer.
        /// </summary>
        /// <param name="pattern">The input pattern.</param>
        /// <returns>The output from this layer.</returns>
        INeuralData Compute(INeuralData pattern);

        /// <summary>
        /// The activation function used for this layer.
        /// </summary>
        IActivationFunction ActivationFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Set or get the neuron count, this will NOT adjust the synapses, or thresholds
        /// other code must do that.
        /// </summary>
        int NeuronCount
        {
            get;
            set;
        }

        /// <summary>
        /// Get a list of all of the outbound synapse connections from this
        /// layer.
        /// </summary>
        IList<ISynapse> Next
        {
            get;
        }

        /// <summary>
        /// The outbound layers from this layer.
        /// </summary>
        ICollection<ILayer> NextLayers
        {
            get;
        }

        /// <summary>
        /// This layer's threshold values, if present, otherwise
        /// this function returns null.
        /// </summary>
        double[] Threshold
        {
            get;
            set;
        }

        /// <summary>
        /// The x-coordinate that this layer should be displayed
        /// at in a GUI.
        /// </summary>
        int X
        {
            get;
            set;
        }


        /// <summary>
        /// The y-coordinate that this layer should be displayed
        /// at in a GUI.
        /// </summary>
        int Y
        {
            get;
            set;
        }

        /// <summary>
        /// True if this layer has threshold values.
        /// </summary>
        bool HasThreshold
        {
            get;
        }

        /// <summary>
        /// Determine if this layer is connected to another.
        /// </summary>
        /// <param name="layer">The second layer, checked to see if it is connected
        /// to this layer.</param>
        /// <returns>True if the two layers are connected.</returns>
        bool IsConnectedTo(ILayer layer);

        /// <summary>
        /// Process the data before it is modified by this layer. This 
        /// method is useful for the context layer to remember the pattern
        /// it was presented with.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        void Process(INeuralData pattern);

        /// <summary>
        /// Called on recurrent layers to provide recurrent output.  This
        /// is where the context layer will return the patter that it 
        /// previously remembered.
        /// </summary>
        /// <returns>The recurrent output.</returns>
        INeuralData Recur();

    }

}
