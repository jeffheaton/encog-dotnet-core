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
using System.Collections.Generic;
using Encog.ML.Data;

namespace Encog.ML
{
    /// <summary>
    /// Defines a cluster. Usually used with the MLClustering method to break input
    /// into clusters.
    /// </summary>
    ///
    public interface IMLCluster
    {
        /// <value>The data in this cluster.</value>
        IList<IMLData> Data { get; }

        /// <summary>
        /// Add data to this cluster.
        /// </summary>
        ///
        /// <param name="pair">The data to add.</param>
        void Add(IMLData pair);

        /// <summary>
        /// Create a machine learning dataset from the data.
        /// </summary>
        ///
        /// <returns>A dataset.</returns>
        IMLDataSet CreateDataSet();

        /// <summary>
        /// Get the specified data item by index.
        /// </summary>
        ///
        /// <param name="pos">The index of the data item to get.</param>
        /// <returns>The data item.</returns>
        IMLData Get(int pos);


        /// <summary>
        /// Remove the specified item.
        /// </summary>
        ///
        /// <param name="data">The item to remove.</param>
        void Remove(IMLData data);


        /// <returns>The number of items.</returns>
        int Count { get; }
    }
}
