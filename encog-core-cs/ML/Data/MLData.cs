//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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

namespace Encog.ML.Data
{
    /// <summary>
    /// Neural data, basically an array of values.
    /// </summary>
    public interface IMLData : ICloneable
    {
        /// <summary>
        /// Get or set the specified index.
        /// </summary>
        /// <param name="x">The index to access.</param>
        /// <returns></returns>
        double this[int x] { get; set; }

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        double[] Data { get; set; }

        /// <summary>
        /// How many elements in this data structure.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Clear the data to zero values.
        /// </summary>
        void Clear();
    }
}
