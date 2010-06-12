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

namespace Encog.Neural.Networks.Structure
{
    /// <summary>
    /// Used to compare layers and ensure a consistent sort order.
    /// </summary>
    public class LayerComparator : IComparer<ILayer>
    {
        /// <summary>
        /// The structure of the neural network.
        /// </summary>
        private NeuralStructure structure;

        /// <summary>
        /// The current depth.
        /// </summary>
        private CalculateDepth depth;

        /// <summary>
        /// The input layer.
        /// </summary>
        private ILayer inputLayer;

        /// <summary>
        /// The output layer.
        /// </summary>
        private ILayer outputLayer;

        /// <summary>
        /// Construct a level comparator for the specified structure.
        /// </summary>
        /// <param name="structure">The structure of the neural network to compare.</param>
        public LayerComparator(NeuralStructure structure)
        {
            this.structure = structure;
            this.depth = new CalculateDepth(structure.Network);
            this.inputLayer = this.structure.Network.GetLayer(
                    BasicNetwork.TAG_INPUT);
            this.outputLayer = this.structure.Network.GetLayer(
                    BasicNetwork.TAG_OUTPUT);
        }

        /// <summary>
        /// Compare two layers.
        /// </summary>
        /// <param name="layer1">The first layer to compare.</param>
        /// <param name="layer2">The second layer to compare.</param>
        /// <returns>The value 0 if the argument layer is equal to this layer; a value
        /// less than 0 if this layer is less than the argument; and a value
        /// greater than 0 if this layer is greater than the layer argument.</returns>
        public int Compare(ILayer layer1, ILayer layer2)
        {

            int depth1 = this.depth.GetDepth(layer1);
            int depth2 = this.depth.GetDepth(layer2);

            // are they the same layers?
            if (layer1 == layer2)
            {
                return 0;
            }
            else if ((layer1 == this.outputLayer)
                    || (layer2 == this.inputLayer))
            {
                return -1;
            }
            else if ((layer2 == this.outputLayer)
                  || (layer1 == this.inputLayer))
            {
                return 1;
            }
            else if (depth1 != depth2)
            {
                return depth1 - depth2;
                // failing all else, just sort them by their ids
            }
            else
            {
                return layer1.ID - layer2.ID;
            }
        }
    }
}
