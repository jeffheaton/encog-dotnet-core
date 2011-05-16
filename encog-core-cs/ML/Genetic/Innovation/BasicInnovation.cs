namespace Encog.ML.Genetic.Innovation
{
    /// <summary>
    /// Provides basic functionality for an innovation.
    /// </summary>
    ///
    public class BasicInnovation : IInnovation
    {
        #region IInnovation Members

        /// <summary>
        /// Set the innovation id.
        /// </summary>
        ///
        /// <value>The innovation id.</value>
        public long InnovationID { /// <returns>The innovation ID.</returns>
            get; /// <summary>
            /// Set the innovation id.
            /// </summary>
            ///
            /// <param name="theInnovationID">The innovation id.</param>
            set; }

        #endregion
    }
}