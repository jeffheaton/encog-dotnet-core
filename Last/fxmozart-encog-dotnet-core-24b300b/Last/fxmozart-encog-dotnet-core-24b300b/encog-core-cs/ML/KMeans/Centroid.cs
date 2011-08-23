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
namespace Encog.ML.Kmeans
{
    /// <summary>
    /// The centers of each cluster.
    /// </summary>
    ///
    public class Centroid
    {
        /// <summary>
        /// The center for each dimension in the input.
        /// </summary>
        ///
        private readonly double[] _centers;

        /// <summary>
        /// The cluster.
        /// </summary>
        ///
        private KMeansCluster _cluster;

        /// <summary>
        /// Construct the centroid.
        /// </summary>
        ///
        /// <param name="theCenters">The centers.</param>
        public Centroid(double[] theCenters)
        {
            _centers = theCenters;
        }


        /// <value>The centers.</value>
        public double[] Centers
        {
            get { return _centers; }
        }


        /// <summary>
        /// Set the cluster.
        /// </summary>
        ///
        /// <value>The cluster.</value>
        public KMeansCluster Cluster
        {
            get { return _cluster; }
            set { _cluster = value; }
        }

        /// <summary>
        /// Calculate the centroid.
        /// </summary>
        ///
        public void CalcCentroid()
        {
            // only called by CAInstance
            int numDp = _cluster.Size();

            var temp = new double[_centers.Length];

            // caluclating the new Centroid
            for (int i = 0; i < numDp; i++)
            {
                for (int j = 0; j < temp.Length; j++)
                {
                    temp[j] += _cluster.Get(i)[j];
                }
            }

            for (int i = 0; i < temp.Length; i++)
            {
                _centers[i] = temp[i]/numDp;
            }

            _cluster.CalcSumOfSquares();
        }
    }
}
