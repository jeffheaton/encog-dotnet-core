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
using Encog.ML.Data.Basic;
using Encog.Util.KMeans;

namespace Encog.ML.KMeans
{
    /// <summary>
    /// Holds a cluster of MLData items that have been clustered 
    /// by the KMeansClustering class.
    /// </summary>
    public class BasicCluster : IMLCluster
    {
        /// <summary>
        /// The contents of the cluster.
        /// </summary>
        private readonly IList<IMLData> _data = new List<IMLData>();

        /// <summary>
        /// Construct a cluster from another. 
        /// </summary>
        /// <param name="cluster">The other cluster.</param>
        public BasicCluster(Cluster<BasicMLDataPair> cluster)
        {
            Centroid = (BasicMLDataPairCentroid) cluster.Centroid();
            foreach (IMLDataPair pair in cluster.Contents)
            {
                _data.Add(pair.Input);
            }
        }

        /// <summary>
        /// The centroid.
        /// </summary>
        public BasicMLDataPairCentroid Centroid { get; set; }

        #region IMLCluster Members

        /// <summary>
        /// Add to the cluster. 
        /// </summary>
        /// <param name="pair">The pair to add.</param>
        public void Add(IMLData pair)
        {
            _data.Add(pair);
        }

        /// <summary>
        /// Create a dataset from the clustered data. 
        /// </summary>
        /// <returns>The dataset.</returns>
        public IMLDataSet CreateDataSet()
        {
            var result = new BasicMLDataSet();

            foreach (IMLData dataItem in _data)
            {
                result.Add(dataItem);
            }

            return result;
        }

        /// <inheritdoc/>
        public IMLData Get(int pos)
        {
            return _data[pos];
        }

        /// <inheritdoc/>
        public IList<IMLData> Data
        {
            get { return _data; }
        }

        /// <inheritdoc/>
        public void Remove(IMLData pair)
        {
            _data.Remove(pair);
        }

        /// <inheritdoc/>
        public int Count
        {
            get { return _data.Count; }
        }

        #endregion
    }
}
