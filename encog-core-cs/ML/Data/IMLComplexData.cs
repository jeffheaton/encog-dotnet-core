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
using Encog.MathUtil;

namespace Encog.ML.Data
{
    /// <summary>
    /// This class implements a data object that can hold complex numbers.  It 
    /// implements the interface MLData, so it can be used with nearly any Encog 
    /// machine learning method.  However, not all Encog machine learning methods 
    /// are designed to work with complex numbers.  A Encog machine learning method 
    /// that does not support complex numbers will only be dealing with the 
    /// real-number portion of the complex number.
    /// </summary>
    public interface IMLComplexData : IMLData
    {
        /// <summary>
        /// The complex numbers.
        /// </summary>
        ComplexNumber[] ComplexData { get; }
        
        /// <summary>
        /// Get the complex data at the specified index. 
        /// </summary>
        /// <param name="index">The index to get the complex data at.</param>
        /// <returns>The complex data.</returns>
        ComplexNumber GetComplexData(int index);

        /// <summary>
        /// Set the complex number array.
        /// </summary>
        /// <param name="theData">The new array.</param>
        void SetComplexData(ComplexNumber[] theData);

        /// <summary>
        /// Set a data element to a complex number.
        /// </summary>
        /// <param name="index">The index to set.</param>
        /// <param name="d">The complex number.</param>
        void SetComplexData(int index, ComplexNumber d);
    }
}
