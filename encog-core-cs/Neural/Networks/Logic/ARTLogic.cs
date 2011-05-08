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
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Neural logic for all ART type networks.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class ARTLogic : INeuralLogic
    {
        /// <summary>
        /// Neural network property, the A1 parameter.
        /// </summary>
        public const String PROPERTY_A1 = "A1";

        /// <summary>
        /// Neural network property, the B1 parameter.
        /// </summary>
        public const String PROPERTY_B1 = "B1";

        /// <summary>
        /// Neural network property, the C1 parameter.
        /// </summary>
        public const String PROPERTY_C1 = "C1";

        /// <summary>
        /// Neural network property, the D1 parameter.
        /// </summary>
        public const String PROPERTY_D1 = "D1";

        /// <summary>
        /// Neural network property, the L parameter.
        /// </summary>
        public const String PROPERTY_L = "L";

        /// <summary>
        /// Neural network property, the vigilance parameter.
        /// </summary>
        public const String PROPERTY_VIGILANCE = "VIGILANCE";

        /// <summary>
        /// The network.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// Setup the network logic, read parameters from the network.
        /// </summary>
        /// <param name="network">The network that this logic class belongs to.</param>
        public virtual void Init(BasicNetwork network)
        {
            this.network = network;
        }

        /// <summary>
        /// The network in use.
        /// </summary>
        public BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// Compute the output for the BasicNetwork class.
        /// </summary>
        /// <param name="input">The input to the network.</param>
        /// <param name="useHolder">The NeuralOutputHolder to use.</param>
        /// <returns>The output from the network.</returns>
        public abstract INeuralData Compute(INeuralData input,
                NeuralOutputHolder useHolder);
    }
}
