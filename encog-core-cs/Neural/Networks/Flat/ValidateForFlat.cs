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
using Encog.Neural.Activation;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// Only certain types of networks can be converted to a flat network.
    /// This class validates this.  Specifically the network must be:
    /// 
    /// 1. Feedforward only, no self-connections or recurrent links
    /// 2. Sigmoid, TANH or linear activation only
    /// 3. Must have bias weight values
    /// </summary>
    public class ValidateForFlat
    {
        /// <summary>
        /// Validate the specified network. 
        /// </summary>
        /// <param name="network">The network to validate.</param>
        public static void ValidateNetwork(BasicNetwork network)
        {
            String str = CanBeFlat(network);
            if (str != null)
                throw new NeuralNetworkError(str);
        }

        /// <summary>
        /// Determine if the specified neural network can be flat.  If it can 
        /// a null is returned, otherwise, an error is returned to show why the 
        /// network cannot be flattened.
        /// </summary>
        /// <param name="network">The network to check.</param>
        /// <returns>Null, if the net can not be flattened, an error 
        /// message otherwise.</returns>
        public static String CanBeFlat(BasicNetwork network)
        {
            ILayer inputLayer = network.GetLayer(BasicNetwork.TAG_INPUT);
            ILayer outputLayer = network.GetLayer(BasicNetwork.TAG_INPUT);

            if (inputLayer == null)
                return "To convert to a flat network, there must be an input layer.";

            if (outputLayer == null)
                return "To convert to a flat network, there must be an output layer.";

            if (network.Structure.IsRecurrent())
            {
                return "To convert to a flat network there cannot be context layers.";
            }

            foreach (ILayer layer in network.Structure.Layers)
            {
                // only feedforward
                if (layer.Next.Count > 1)
                {
                    return "To convert to flat a network must be feedforward only.";
                }

                if (!(layer.ActivationFunction is ActivationSigmoid)
                        && !(layer.ActivationFunction is ActivationTANH))
                {
                    return "To convert to flat a network must only use sigmoid, linear or tanh activation.";
                }

                if (!layer.HasBias && layer!=inputLayer )
                {
                    return "To convert to flat, all non-input layers must have bias weight values.";
                }
            }
            return null;
        }
    }
}

