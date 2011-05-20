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
    public class KMeansCluster : MLCluster
    {
        /// <summary>
        /// The contents of the cluster.
        /// </summary>
        ///
        private readonly IList<MLData> data;

        /// <summary>
        /// The centroid.
        /// </summary>
        ///
        private Centroid centroid;

        /// <summary>
        /// The sum square.
        /// </summary>
        ///
        private double sumSqr;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public KMeansCluster()
        {
            data = new List<MLData>();
        }

        /// <summary>
        /// Set the centroid.
        /// </summary>
        public Centroid Centroid
        {
            get { return centroid; }
            set { centroid = value; }
        }

        /// <value>The sum of squares.</value>
        public double SumSqr
        {
            get { return sumSqr; }
        }

        #region MLCluster Members

        /// <summary>
        /// Add to the cluster.
        /// </summary>
        ///
        /// <param name="pair">The pair to add.</param>
        public void Add(MLData pair)
        {
            data.Add(pair);
            CalcSumOfSquares();
        }

        /// <summary>
        /// Create a dataset from the clustered data.
        /// </summary>
        ///
        /// <returns>The dataset.</returns>
        public MLDataSet CreateDataSet()
        {
            MLDataSet result = new BasicMLDataSet();


            foreach (MLData dataItem  in  data)
            {
                result.Add(dataItem);
            }

            return result;
        }

        /// <inheritdoc/>
        public MLData Get(int pos)
        {
            return data[pos];
        }


        /// <inheritdoc/>
        public IList<MLData> Data
        {
            get { return data; }
        }


        /// <inheritdoc/>
        public void Remove(MLData pair)
        {
            data.Remove(pair);
            CalcSumOfSquares();
        }

        /// <inheritdoc/>
        public int Size()
        {
            return data.Count;
        }

        #endregion

        /// <summary>
        /// Calculate the sum of squares.
        /// </summary>
        public void CalcSumOfSquares()
        {
            int size = data.Count;
            double temp = 0;
            for (int i = 0; i < size; i++)
            {
                temp += KMeansClustering.CalculateEuclideanDistance(centroid,
                                                                    (data[i]));
            }
            sumSqr = temp;
        }
    }
}
