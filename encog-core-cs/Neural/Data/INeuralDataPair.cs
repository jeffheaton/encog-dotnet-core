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
using Encog.Neural.Data;

namespace Encog.Neural.NeuralData
{
    /// <summary>
    /// A neural data pair holds both the input and ideal data.  If this
    /// is an unsupervised data element, then only input is provided.
    /// </summary>
    public interface INeuralDataPair: ICloneable
    {
        /// <summary>
        /// The input that the neural network.
        /// </summary>
        INeuralData Input
        {
            get;
        }

        /// <summary>
        /// The ideal data that the neural network should produce
        /// for the specified input.
        /// </summary>
        INeuralData Ideal
        {
            get;
        }

        /// <summary>
        /// True if this training pair is supervised.  That is, it has 
	    /// both input and ideal data.
        /// </summary>
        bool IsSupervised
        {
            get;
        }
    }
}
