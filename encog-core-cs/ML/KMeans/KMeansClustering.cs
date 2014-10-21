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
using Encog.ML.Data.Basic;
using Encog.ML.KMeans;
using Encog.Util.KMeans;

namespace Encog.ML.Kmeans
{
    /// <summary>
    /// This class performs a basic K-Means clustering. This class can be used on
    /// either supervised or unsupervised data. For supervised data, the ideal values
    /// will be ignored.
    /// http://en.wikipedia.org/wiki/Kmeans
    /// </summary>
    ///
    public class KMeansClustering : IMLClustering
    {
        /// <summary>
        /// Number of clusters.
        /// </summary>
        private readonly int _k;

        /// <summary>
        /// The kmeans utility.
        /// </summary>
        private readonly KMeansUtil<BasicMLDataPair> _kmeans;

        /// <summary>
        /// The clusters
        /// </summary>
        private IMLCluster[] _clusters;

        /// <summary>
        /// Construct the K-Means object.
        /// </summary>
        /// <param name="theK">The number of clusters to use.</param>
        /// <param name="theSet">The dataset to cluster.</param>
        public KMeansClustering(int theK, IMLDataSet theSet)
        {
            IList<BasicMLDataPair> list = new List<BasicMLDataPair>();
            foreach (IMLDataPair pair in theSet)
            {
                list.Add((BasicMLDataPair) pair);
            }
            _k = theK;
            _kmeans = new KMeansUtil<BasicMLDataPair>(_k, list as IList);
        }

        #region IMLClustering Members

        /// <summary>
        /// Perform a single training iteration.
        /// </summary>
        public void Iteration()
        {
            _kmeans.Process();
            _clusters = new IMLCluster[_k];
            for (int i = 0; i < _k; i++)
            {
                _clusters[i] = new BasicCluster(_kmeans.GetCluster(i));
            }
        }

        /// <summary>
        /// The number of iterations to perform.
        /// </summary>
        /// <param name="count">The count of iterations.</param>
        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }


        /// <summary>
        /// The clusters.
        /// </summary>
        public IMLCluster[] Clusters
        {
            get { return _clusters; }
        }


        /// <summary>
        /// The number of clusters.
        /// </summary>
        public int Count
        {
            get { return _k; }
        }

        #endregion
    }
}
