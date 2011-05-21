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
using System.Collections.Generic;
using Encog.ML.Data;
using Encog.ML.Data.Basic;

namespace Encog.ML.Kmeans
{
    /// <summary>
    /// Holds a cluster of MLData items that have been clustered 
    /// by the KMeansClustering class.
    /// </summary>
    ///
    public class KMeansCluster : IMLCluster
    {
        /// <summary>
        /// The contents of the cluster.
        /// </summary>
        ///
        private readonly IList<IMLData> _data;

        /// <summary>
        /// The centroid.
        /// </summary>
        ///
        private Centroid _centroid;

        /// <summary>
        /// The sum square.
        /// </summary>
        ///
        private double _sumSqr;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public KMeansCluster()
        {
            _data = new List<IMLData>();
        }

        /// <summary>
        /// Set the centroid.
        /// </summary>
        public Centroid Centroid
        {
            get { return _centroid; }
            set { _centroid = value; }
        }

        /// <value>The sum of squares.</value>
        public double SumSqr
        {
            get { return _sumSqr; }
        }

        #region MLCluster Members

        /// <summary>
        /// Add to the cluster.
        /// </summary>
        ///
        /// <param name="pair">The pair to add.</param>
        public void Add(IMLData pair)
        {
            _data.Add(pair);
            CalcSumOfSquares();
        }

        /// <summary>
        /// Create a dataset from the clustered data.
        /// </summary>
        ///
        /// <returns>The dataset.</returns>
        public IMLDataSet CreateDataSet()
        {
            IMLDataSet result = new BasicMLDataSet();


            foreach (IMLData dataItem  in  _data)
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
            CalcSumOfSquares();
        }

        /// <inheritdoc/>
        public int Size()
        {
            return _data.Count;
        }

        #endregion

        /// <summary>
        /// Calculate the sum of squares.
        /// </summary>
        public void CalcSumOfSquares()
        {
            int size = _data.Count;
            double temp = 0;
            for (int i = 0; i < size; i++)
            {
                temp += KMeansClustering.CalculateEuclideanDistance(_centroid,
                                                                    (_data[i]));
            }
            _sumSqr = temp;
        }
    }
}
