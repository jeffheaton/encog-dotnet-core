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
using Encog.ML.Data;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Data;
#if logging
using log4net;
#endif

namespace Encog.Neural.Networks
{
    /// <summary>
    /// Holds the output from each layer of the neural network. This is very useful
 /// for the propagation algorithms that need to examine the output of each
 /// individual layer.
    /// </summary>
    public class NeuralOutputHolder
    {

        /// <summary>
        /// The results from each of the synapses.
        /// </summary>
        private IDictionary<ISynapse, MLData> result;

        /// <summary>
        /// The output from the entire neural network.
        /// </summary>
        private MLData output;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(NeuralOutputHolder));
#endif

        /// <summary>
        /// Construct an empty holder.
        /// </summary>
        public NeuralOutputHolder()
        {
            this.result = new Dictionary<ISynapse, MLData>();
        }

        /// <summary>
        /// The output from the neural network.
        /// </summary>
        public MLData Output
        {
            get
            {
                return this.output;
            }
            set
            {
                this.output = value;
            }
        }

        /// <summary>
        /// The result from the synapses in a map.
        /// </summary>
        public IDictionary<ISynapse, MLData> Result
        {
            get
            {
                return this.result;
            }
        }
    }
}
