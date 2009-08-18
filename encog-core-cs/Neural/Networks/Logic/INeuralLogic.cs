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
using System.Runtime.Serialization;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Neural logic classes implement neural network logic for a variety
    /// of neural network setups.
    /// </summary>
    public interface INeuralLogic 
    {
        /// <summary>
        /// Compute the output for the BasicNetwork class.
        /// </summary>
        /// <param name="input">The input to the network.</param>
        /// <param name="useHolder">The NeuralOutputHolder to use.</param>
        /// <returns>The output from the network.</returns>
        INeuralData Compute(INeuralData input,
                NeuralOutputHolder useHolder);

        /// <summary>
        /// Setup the network logic, read parameters from the network.
        /// </summary>
        /// <param name="network">The network that this logic class belongs to.</param>
        void Init(BasicNetwork network);
    }
}
