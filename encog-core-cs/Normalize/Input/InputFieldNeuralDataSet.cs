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
using Encog.Neural.NeuralData;
using Encog.Persist.Attributes;

namespace Encog.Normalize.Input
{
    /// <summary>
    /// An input field based on an Encog NeuralDataSet.
    /// </summary>
    [EGUnsupported]
    public class InputFieldNeuralDataSet : BasicInputField
    {
        /// <summary>
        /// The data set.
        /// </summary>
        private INeuralDataSet data;

        /// <summary>
        /// The input or ideal index.  This treats the input and ideal as one
        /// long array, concatenated together.
        /// </summary>
        private int offset;

        /// <summary>
        /// Construct a input field based on a NeuralDataSet.
        /// </summary>
        /// <param name="usedForNetworkInput">Is this field used for neural input.</param>
        /// <param name="data">The data set to use.</param>
        /// <param name="offset">The input or ideal index to use. This treats the input 
        /// and ideal as one long array, concatenated together.</param>
        public InputFieldNeuralDataSet(bool usedForNetworkInput,
                 INeuralDataSet data, int offset)
        {
            this.data = data;
            this.offset = offset;
            UsedForNetworkInput = usedForNetworkInput;
        }

        /// <summary>
        /// The neural data set to read.
        /// </summary>
        public INeuralDataSet NeuralDataSet
        {
            get
            {
                return this.data;
            }
        }

        /// <summary>
        /// The field to be accessed. This treats the input and 
        /// ideal as one long array, concatenated together.
        /// </summary>
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

    }
}
