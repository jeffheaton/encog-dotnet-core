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
using System.Collections;
using System.Collections.Generic;
using Encog.ML.Data;
using Encog.Util.KMeans;

namespace Encog.ML.HMM.Train.KMeans
{
    /// <summary>
    /// Clusters used for the KMeans HMM training algorithm.
    /// </summary>
    public class Clusters
    {
        /// <summary>
        /// A list of all of the clusters.
        /// </summary>
        private readonly IList<ICollection<IMLDataPair>> _clusters;

        /// <summary>
        /// Provide quick access to the clusters.
        /// </summary>
        private readonly IDictionary<IMLDataPair, int> _clustersHash;

        /// <summary>
        /// Construct the clusters objects. 
        /// </summary>
        /// <param name="k">The number of clusters to have.</param>
        /// <param name="observations">The observations.</param>
        public Clusters(int k, IMLDataSet observations)
        {
            _clustersHash = new Dictionary<IMLDataPair, int>();
            _clusters = new List<ICollection<IMLDataPair>>();

            IList list = new List<IMLDataPair>();
            foreach (IMLDataPair pair in observations)
            {
                list.Add(pair);
            }
            var kmc = new KMeansUtil<IMLDataPair>(k, list);
            kmc.Process();

            for (int i = 0; i < k; i++)
            {
                ICollection<IMLDataPair> cluster = kmc.Get(i);
                _clusters.Add(cluster);

                foreach (IMLDataPair element in cluster)
                {
                    _clustersHash[element] = i;
                }
            }
        }

        /// <summary>
        /// Get the speicified cluster. 
        /// </summary>
        /// <param name="n">The number.</param>
        /// <returns>The items in that cluster.</returns>
        public ICollection<IMLDataPair> Cluster(int n)
        {
            return _clusters[n];
        }

        /// <summary>
        /// Get the cluster for the specified data pair. 
        /// </summary>
        /// <param name="o">The data pair to use..</param>
        /// <returns>The cluster the pair is in.</returns>
        public int Cluster(IMLDataPair o)
        {
            return _clustersHash[o];
        }

        /// <summary>
        /// Determine if the specified object is in one of the clusters. 
        /// </summary>
        /// <param name="o">The object to check.</param>
        /// <param name="x">The cluster.</param>
        /// <returns>True if the object is in the cluster.</returns>
        public bool IsInCluster(IMLDataPair o, int x)
        {
            return Cluster(o) == x;
        }

        /// <summary>
        /// Put an object into the specified cluster. 
        /// </summary>
        /// <param name="o">The object.</param>
        /// <param name="n">The cluster number.</param>
        public void Put(IMLDataPair o, int n)
        {
            _clustersHash[o] = n;
            _clusters[n].Add(o);
        }

        /// <summary>
        /// Remove an object from the specified cluster. 
        /// </summary>
        /// <param name="o">The object to remove.</param>
        /// <param name="n">The cluster to remove from.</param>
        public void Remove(IMLDataPair o, int n)
        {
            _clustersHash[o] = -1;
            _clusters[n].Remove(o);
        }
    }
}
