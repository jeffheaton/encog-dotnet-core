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

namespace Encog.Normalize.Input
{
    /// <summary>
    ///  * A Normalization input field.  This field defines data that needs to be 
    /// normalized.  There are many different types of normalization field that can
    /// be used for many different purposes.
    /// 
    /// To assist in normalization each input file tracks the min and max values for
    /// that field.
    /// </summary>
    public interface IInputField
    {
        /// <summary>
        /// Update the min and max values for this field with the specified values.
        /// </summary>
        /// <param name="d">The current value to use to update min and max.</param>
        void ApplyMinMax(double d);

        /// <summary>
        /// The current value of the input field.  This is only valid, 
        /// while the normalization is being performed.
        /// </summary>
        double CurrentValue { get; set; }

        /// <summary>
        /// The maximum value for all of the input data, this is calculated
        /// during the first pass of normalization.
        /// </summary>
        double Max { get; set; }

        /// <summary>
        /// The minimum value for all of the input data, this is calculated
        /// during the first pass of normalization.
        /// </summary>
        double Min { get; set; }

        /// <summary>
        /// True, if this field is used for network input.  This is needed
        /// so that the buildForNetworkInput method of the normalization class knows
        /// how many input fields to expect.  For instance, fields used only to 
        /// segregate data are not used for the actual network input and may
        /// not be provided when the network is actually being queried.
        /// </summary>
        bool UsedForNetworkInput { get; }

        /// <summary>
        /// Called for input field types that require an index to get the current
        /// value. This is used by the InputFieldArray1D and InputFieldArray2D
        /// classes.
        /// </summary>
        /// <param name="i">The index to read.</param>
        /// <returns>The value read.</returns>
        double GetValue(int i);


    }
}
