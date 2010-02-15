// Encog(tm) Artificial Intelligence Framework v2.3
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
    /// Utility class to calculate the depth that a layer is from the output layer.
    /// If there are multiple ways to get to the specified layer, then the longest
    /// depth is returned.  This class is used by propagation training to ensure
    /// that the layers are always returned on a consistent order.
    /// </summary>
    public class CalculateDepth
    {
        /// <summary>
        /// The depth so far at each layer.
        /// </summary>
        private IDictionary<ILayer, int> depths = new Dictionary<ILayer, int>();

        /// <summary>
        /// The network.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The output layer.
        /// </summary>
        private ILayer outputLayer;

        /// <summary>
        /// Construct the depth calculation object.
        /// </summary>
        /// <param name="network">The network that we are calculating for.</param>
        public CalculateDepth(BasicNetwork network)
        {
            this.network = network;
            this.outputLayer = network.GetLayer(BasicNetwork.TAG_OUTPUT);
            if( this.outputLayer!=null )
                Calculate(0, this.outputLayer);
        }


        /// <summary>
        /// Called internally to calculate a depth. 
        /// </summary>
        /// <param name="currentDepth">The current depth.</param>
        /// <param name="layer">The layer we are on.</param>
        private void Calculate(int currentDepth, ILayer layer)
        {

            // record this layer
            if (this.depths.ContainsKey(layer))
            {
                int oldDepth = this.depths[layer];
                if (currentDepth > oldDepth)
                {
                    this.depths[layer] = currentDepth;
                }
            }
            else
            {
                this.depths[layer] = currentDepth;
            }

            // traverse all of the ways to get to that layer
            ICollection<ILayer> prev = this.network.Structure
                    .GetPreviousLayers(this.outputLayer);

            foreach (ILayer nextLayer in prev)
            {
                if (!this.depths.ContainsKey(nextLayer))
                {
                    Calculate(currentDepth + 1, nextLayer);
                }
            }
        }

        /// <summary>
        /// Get the depth for a specific layer.
        /// </summary>
        /// <param name="layer">The layer to get the depth for.</param>
        /// <returns>The depth of the specified layer.</returns>
        public int GetDepth(ILayer layer)
        {
            if (!this.depths.ContainsKey(layer))
            {
                return -1;
            }
            else
            {
                return this.depths[layer];
            }
        }
    }
}
