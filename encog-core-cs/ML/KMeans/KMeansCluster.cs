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