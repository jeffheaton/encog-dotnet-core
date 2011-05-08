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
using Encog.Neural.Data;
using Encog.Neural.Networks.Synapse;

#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Provides the neural logic for an Simple Recurrent Network (SRN) type network.  
    /// This class is used for the Elman and Jordan networks.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class SimpleRecurrentLogic : FeedforwardLogic
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(SimpleRecurrentLogic));
#endif
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
                 MLData input, ISynapse source)
        {
            foreach (ISynapse synapse in
                     this.Network.Structure.GetPreviousSynapses(layer))
            {
                if (synapse != source)
                {
#if logging
                    if (SimpleRecurrentLogic.logger.IsDebugEnabled)
                    {
                        SimpleRecurrentLogic.logger.Debug("Recurrent layer from: " + input.ToString());
                    }
#endif
                    MLData recurrentInput = synapse.FromLayer.Recur();

                    if (recurrentInput != null)
                    {
                        MLData recurrentOutput = synapse
                               .Compute(recurrentInput);

                        for (int i = 0; i < input.Count; i++)
                        {
                            input[i] = input[i]
                                    + recurrentOutput[i];
                        }
#if logging
                        if (SimpleRecurrentLogic.logger.IsDebugEnabled)
                        {
                            SimpleRecurrentLogic.logger.Debug("Recurrent layer to: " + input.ToString());
                        }
#endif
                    }
                }
            }
        }
    }
}
