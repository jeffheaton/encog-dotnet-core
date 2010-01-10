// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
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
    /// Provides basic functionality, such as min/max and current value
    /// for other input fields.
    /// </summary>
    [EGReferenceable]
    public class BasicInputField : IInputField
    {
        /// <summary>
        /// The maximum value encountered so far for this field.
        /// </summary>
        [EGAttribute]
        private double min = Double.PositiveInfinity;

        /// <summary>
        /// The minimum value encountered so far for this field.
        /// </summary>
        [EGAttribute]
        private double max = Double.NegativeInfinity;

        /// <summary>
        /// The current value for this field, only used while normalizing.
        /// </summary>
        [EGIgnore]
        private double currentValue;

        /// <summary>
        /// True if this field is used to actually generate the input for
        /// the neural network.
        /// </summary>
        [EGAttribute]
        private bool usedForNetworkInput = true;

        /// <summary>
        /// Given the current value, apply to the min and max values.
        /// </summary>
        /// <param name="d">The current value.</param>
        public void ApplyMinMax(double d)
        {
            this.min = Math.Min(this.min, d);
            this.max = Math.Max(this.max, d);
        }


        /// <summary>
        /// The current value of the input field.  This is only valid, 
        /// while the normalization is being performed.
        /// </summary>
        public double CurrentValue
        {
            get
            {
                return this.currentValue;
            }
            set
            {
                this.currentValue = value;
            }
        }

        /// <summary>
        /// The maximum value for all of the input data, this is calculated
        /// during the first pass of normalization.
        /// </summary>
        public double Max
        {
            get
            {
                return this.max;
            }
            set
            {
                this.max = value;
            }
        }

        /// <summary>
        /// The minimum value for all of the input data, this is calculated
        /// during the first pass of normalization.
        /// </summary>
        public double Min
        {
            get
            {
                return this.min;
            }
            set
            {
                this.min = value;
            }
        }

        /// <summary>
        /// Not supported for this sort of class, may be implemented in subclasses.
        /// Will throw an exception.
        /// </summary>
        /// <param name="i">The index.  Not used.</param>
        /// <returns>The value at the specified index.</returns>
        public virtual double GetValue(int i)
        {
            throw new NormalizationError("Can't call getValue on "
                    + this.GetType().Name);
        }


        /// <summary>
        /// True, if this field is used for network input.  
        /// This is needed so that the buildForNetworkInput method of the 
        /// normalization class knows how many input fields to expect.  For instance, 
        /// fields used only to segregate data are not used for the actual network 
        /// input and may not be provided when the network is actually being queried.
        /// </summary>
        public bool UsedForNetworkInput
        {
            get
            {
                return this.usedForNetworkInput;
            }
            set
            {
                this.usedForNetworkInput = value;
            }
        }
    }
}
