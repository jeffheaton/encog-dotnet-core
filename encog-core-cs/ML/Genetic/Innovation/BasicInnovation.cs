using System;
namespace Encog.ML.Genetic.Innovation
{
    /// <summary>
    /// Provides basic functionality for an innovation.
    /// </summary>
    [Serializable]
    public class BasicInnovation : IInnovation
    {
        #region IInnovation Members

        /// <summary>
        /// Set the innovation id.
        /// </summary>
        public long InnovationID { 
            get;
            set; }

        #endregion
    }
}