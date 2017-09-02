//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;

namespace Encog.Util.Normalize.Input
{
    /// <summary>
    /// Provides basic functionality, such as min/max and current value
    /// for other input fields.
    /// </summary>
    [Serializable]
    public class BasicInputField : IInputField
    {
        /// <summary>
        /// The minimum value encountered so far for this field.
        /// </summary>
        private double _max = Double.NegativeInfinity;

        /// <summary>
        /// The maximum value encountered so far for this field.
        /// </summary>
        private double _min = Double.PositiveInfinity;

        /// <summary>
        /// True if this field is used to actually generate the input for
        /// the neural network.
        /// </summary>
        private bool _usedForNetworkInput = true;

        #region IInputField Members

        /// <summary>
        /// Given the current value, apply to the min and max values.
        /// </summary>
        /// <param name="d">The current value.</param>
        public void ApplyMinMax(double d)
        {
            _min = Math.Min(_min, d);
            _max = Math.Max(_max, d);
        }


        /// </inheritdoc>
        public double CurrentValue { get; set; }

        /// </inheritdoc>
        public object CurrentValueRaw { get; set; }

        /// <summary>
        /// The maximum value for all of the input data, this is calculated
        /// during the first pass of normalization.
        /// </summary>
        public double Max
        {
            get { return _max; }
            set { _max = value; }
        }

        /// <summary>
        /// The minimum value for all of the input data, this is calculated
        /// during the first pass of normalization.
        /// </summary>
        public double Min
        {
            get { return _min; }
            set { _min = value; }
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
                                         + GetType().Name);
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
            get { return _usedForNetworkInput; }
            set { _usedForNetworkInput = value; }
        }

        #endregion
    }
}
