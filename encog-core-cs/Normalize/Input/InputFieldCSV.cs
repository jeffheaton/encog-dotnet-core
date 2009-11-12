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
using Encog.Persist.Attributes;

namespace Encog.Normalize.Input
{
    /// <summary>
    /// An input field based on a CSV file.
    /// </summary>
    [EGReferenceable]
    public class InputFieldCSV : BasicInputField
    {
        /// <summary>
        /// The file to read.
        /// </summary>
        private String file;

        /// <summary>
        /// The CSV column represented by this field.
        /// </summary>
        [EGAttribute]
        private int offset;

        /// <summary>
        /// Construct an InputFieldCSV with the default constructor.  This is mainly
        /// used for reflection.
        /// </summary>
        public InputFieldCSV()
        {

        }

        /// <summary>
        /// Construct a input field for a CSV file.
        /// </summary>
        /// <param name="usedForNetworkInput">True if this field is used for actual 
        /// input to the neural network, as opposed to segregation only.</param>
        /// <param name="file">The tile to read.</param>
        /// <param name="offset">The CSV file column to read.</param>
        public InputFieldCSV(bool usedForNetworkInput, String file,
                 int offset)
        {
            this.file = file;
            this.offset = offset;
            UsedForNetworkInput = usedForNetworkInput;
        }

        /// <summary>
        /// The file being read.
        /// </summary>
        public String File
        {
            get
            {
                return this.file;
            }
        }

        /// <summary>
        /// The column in this CSV file to read.
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
