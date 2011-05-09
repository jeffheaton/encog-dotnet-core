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
using Encog.Engine.Network.Flat;
using Encog.Util;

namespace Encog.Neural.Networks.Training.Cross
{
    /// <summary>
    /// The network for one fold of a cross validation.
    /// </summary>
    public class NetworkFold
    {
        /// <summary>
        /// The weights for this fold.
        /// </summary>
        private double[] weights;

        /// <summary>
        /// The output for this fold.
        /// </summary>
        private double[] output;
        
        /// <summary>
        /// Construct a fold from the specified flat network. 
        /// </summary>
        /// <param name="flat">The flat network.</param>
        public NetworkFold(FlatNetwork flat)
        {
            this.weights = EngineArray.ArrayCopy(flat.Weights);
            this.output = EngineArray.ArrayCopy(flat.LayerOutput);
        }

        /// <summary>
        /// Copy weights and output to the network. 
        /// </summary>
        /// <param name="target">The network to copy to.</param>
        public void CopyToNetwork(FlatNetwork target)
        {
            EngineArray.ArrayCopy(this.weights, target.Weights);
            EngineArray.ArrayCopy(this.output, target.LayerOutput);
        }

        /// <summary>
        /// Copy the weights and output from the network. 
        /// </summary>
        /// <param name="source">The network to copy from.</param>
        public void CopyFromNetwork(FlatNetwork source)
        {
            EngineArray.ArrayCopy(source.Weights, this.weights);
            EngineArray.ArrayCopy(source.LayerOutput, this.output);
        }

        /// <summary>
        /// The network weights.
        /// </summary>
        public double[] Weights
        {
            get
            {
                return weights;
            }
        }

        /// <summary>
        /// The network output.
        /// </summary>
        public double[] Output
        {
            get
            {
                return output;
            }
        }

    }
}
