namespace Encog.ML.Genetic.Innovation
{
    /// <summary>
    /// Provides the interface for an innovation. An innovation is a enhancement that
    /// was tried to the genome.
    /// </summary>
    ///
    public interface IInnovation
    {
        /// <summary>
        /// Set the innovation id.
        /// </summary>
        long InnovationID { 
            get;
            set; }
    }
}