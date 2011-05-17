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
        private readonly double[] centers;

        /// <summary>
        /// The cluster.
        /// </summary>
        ///
        private KMeansCluster cluster;

        /// <summary>
        /// Construct the centroid.
        /// </summary>
        ///
        /// <param name="theCenters">The centers.</param>
        public Centroid(double[] theCenters)
        {
            centers = theCenters;
        }


        /// <value>The centers.</value>
        public double[] Centers
        {
            /// <returns>The centers.</returns>
            get { return centers; }
        }


        /// <summary>
        /// Set the cluster.
        /// </summary>
        ///
        /// <value>The cluster.</value>
        public KMeansCluster Cluster
        {
            /// <returns>The clusters.</returns>
            get { return cluster; }
            /// <summary>
            /// Set the cluster.
            /// </summary>
            ///
            /// <param name="c">The cluster.</param>
            set { cluster = value; }
        }

        /// <summary>
        /// Calculate the centroid.
        /// </summary>
        ///
        public void CalcCentroid()
        {
            // only called by CAInstance
            int numDP = cluster.Size();

            var temp = new double[centers.Length];

            // caluclating the new Centroid
            for (int i = 0; i < numDP; i++)
            {
                for (int j = 0; j < temp.Length; j++)
                {
                    temp[j] += cluster.Get(i)[j];
                }
            }

            for (int i_0 = 0; i_0 < temp.Length; i_0++)
            {
                centers[i_0] = temp[i_0]/numDP;
            }

            cluster.CalcSumOfSquares();
        }
    }
}