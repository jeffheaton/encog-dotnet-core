using System.Collections.Generic;

namespace Encog.ML.Genetic.Innovation
{
    /// <summary>
    /// Defines a list of innovations.
    /// </summary>
    ///
    public interface IInnovationList
    {
        /// <value>A list of innovations.</value>
        IList<IInnovation> Innovations { /// <returns>A list of innovations.</returns>
            get; }

        /// <summary>
        /// Add an innovation.
        /// </summary>
        ///
        /// <param name="innovation">The innovation added.</param>
        void Add(IInnovation innovation);

        /// <summary>
        /// Get the innovation specified by index.
        /// </summary>
        ///
        /// <param name="id">The index.</param>
        /// <returns>The innovation.</returns>
        IInnovation Get(int id);
    }
}