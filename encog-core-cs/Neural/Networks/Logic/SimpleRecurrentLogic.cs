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
using Encog.Neural.Networks.Layers;
using Encog.Neural.Data;
using log4net;
using Encog.Neural.Networks.Synapse;

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Provides the neural logic for an Simple Recurrent Network (SRN) type network.  
    /// This class is used for the Elman and Jordan networks.
    /// </summary>
    [Serializable]
    public class SimpleRecurrentLogic : FeedforwardLogic
    {
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(SimpleRecurrentLogic));

        /// <summary>
        /// Handle recurrent layers.  See if there are any recurrent layers before
        /// the specified layer that must affect the input.
        /// </summary>
        /// <param name="layer">The layer being processed, see if there are any recurrent
        /// connections to this.</param>
        /// <param name="input">The input to the layer, will be modified with the result
        /// from any recurrent layers.</param>
        /// <param name="source">The source synapse.</param>
        public override void PreprocessLayer(ILayer layer,
                 INeuralData input, ISynapse source)
        {
            foreach (ISynapse synapse in
                     this.Network.Structure.GetPreviousSynapses(layer))
            {
                if (synapse != source)
                {
                    /*if (SimpleRecurrentLogic.logger.IsDebugEnabled)
                    {
                        SimpleRecurrentLogic.logger.Debug("Recurrent layer from: " + input.ToString());
                    }*/
                    INeuralData recurrentInput = synapse.FromLayer.Recur();

                    if (recurrentInput != null)
                    {
                        INeuralData recurrentOutput = synapse
                               .Compute(recurrentInput);

                        for (int i = 0; i < input.Count; i++)
                        {
                            input[i] = input[i]
                                    + recurrentOutput[i];
                        }

                        /*if (SimpleRecurrentLogic.logger.IsDebugEnabled)
                        {
                            SimpleRecurrentLogic.logger.Debug("Recurrent layer to: " + input.ToString());
                        }*/
                    }
                }
            }
        }
    }
}
