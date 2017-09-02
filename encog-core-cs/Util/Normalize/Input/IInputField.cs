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
namespace Encog.Util.Normalize.Input
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
        /// The current value of the input field as a double.  This is only valid, 
        /// while the normalization is being performed.  This value will be NAN
        /// if it was missing, or could not be read.
        /// </summary>
        double CurrentValue { get; set; }

        /// <summary>
        /// The current value of the input field as it was read, usually double or string.  
        /// This is only valid, while the normalization is being performed.  This value
        /// will be null if it was missing.
        /// </summary>
        object CurrentValueRaw { get; set; }

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
        /// Update the min and max values for this field with the specified values.
        /// </summary>
        /// <param name="d">The current value to use to update min and max.</param>
        void ApplyMinMax(double d);

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
