namespace Encog.ML.Bayesian.Training
{
    /// <summary>
    /// The method by which a Bayesian network should be initialized.
    /// </summary>
    public enum BayesianInit
    {
        /// <summary>
        /// No init, do not change anything.
        /// </summary>
        InitNoChange,

        /// <summary>
        /// Start with no connections.
        /// </summary>
        InitEmpty,

        /// <summary>
        /// Init as Naive Bayes.
        /// </summary>
        InitNaiveBayes
    }
}