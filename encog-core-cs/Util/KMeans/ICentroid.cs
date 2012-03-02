
namespace Encog.Util.KMeans
{
    /// <summary>
    /// A centroid.
    /// </summary>
    /// <typeparam name="TO">The type.</typeparam>
    public interface ICentroid<in TO>
    {        
        /// <summary>
        /// Add an element to the centroid.
        /// </summary>
        /// <param name="e">The element to add.</param>
        void Add(TO e);

        /// <summary>
        /// Remove an element from the centroid. 
        /// </summary>
        /// <param name="o">The element to remove.</param>
        void Remove(TO o);

        /// <summary>
        /// The distance between this centroid and an element. 
        /// </summary>
        /// <param name="o">The element.</param>
        /// <returns>The distance.</returns>
        double Distance(TO o);
    }
}
