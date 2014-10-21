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
using Encog.Util.KMeans;

namespace Encog.ML.Data
{
    /// <summary>
    /// Neural data, basically an array of values.
    /// </summary>
    public interface IMLData : ICloneable, ICentroidFactory<IMLData>
    {
        /// <summary>
        /// Get the specified index.
        /// </summary>
        /// <param name="x">The index to access.</param>
        double this[int x] { get; }

        /// <summary>
        /// How many elements in this data structure.
        /// </summary>
        int Count { get; }

		/// <summary>
		/// Copy the data to the target array. The starting index is implementation-specific.
		/// </summary>
		void CopyTo(double[] target, int targetIndex, int count);
    }

	public interface IMLDataModifiable: IMLData
	{
		/// <summary>
		/// Set the specified index.
		/// </summary>
		/// <param name="x">The index to access.</param>
		new double this[int x] { get; set; }
	}
}
