using System.Collections.Generic;
using Encog.ML.Data;

namespace Encog.ML
{
    /// <summary>
    /// Defines a cluster. Usually used with the MLClustering method to break input
    /// into clusters.
    /// </summary>
    ///
    public interface MLCluster
    {
        /// <value>The data in this cluster.</value>
        IList<MLData> Data { /// <returns>The data in this cluster.</returns>
            get; }

        /// <summary>
        /// Add data to this cluster.
        /// </summary>
        ///
        /// <param name="pair">The data to add.</param>
        void Add(MLData pair);

        /// <summary>
        /// Create a machine learning dataset from the data.
        /// </summary>
        ///
        /// <returns>A dataset.</returns>
        MLDataSet CreateDataSet();

        /// <summary>
        /// Get the specified data item by index.
        /// </summary>
        ///
        /// <param name="pos">The index of the data item to get.</param>
        /// <returns>The data item.</returns>
        MLData Get(int pos);


        /// <summary>
        /// Remove the specified item.
        /// </summary>
        ///
        /// <param name="data">The item to remove.</param>
        void Remove(MLData data);


        /// <returns>The number of items.</returns>
        int Size();
    }
}