using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.KMeans
{
    /// <summary>
    /// An object that can create centroids.
    /// </summary>
    /// <typeparam name="TO">The element type for the centroid.</typeparam>
    public interface ICentroidFactory<in TO>
    {
        /// <summary>
        /// The centroid.
        /// </summary>
        /// <returns>The centroid.</returns>
        ICentroid<TO> CreateCentroid();
    }
}
