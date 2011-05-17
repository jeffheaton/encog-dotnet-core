namespace Encog.App.Analyst.Wizard
{
    /// <summary>
    /// The machine learning method that the Encog Analyst Wizard should use.
    /// </summary>
    ///
    public enum WizardMethodType
    {
        /// <summary>
        /// Feed forward network.
        /// </summary>
        ///
        FeedForward,
        /// <summary>
        /// RBF network.
        /// </summary>
        ///
        RBF,
        /// <summary>
        /// SVM network.
        /// </summary>
        ///
        SVM,
        /// <summary>
        /// NEAT network.
        /// </summary>
        ///
        NEAT,
        /// <summary>
        /// PNN neural network.
        /// </summary>
        ///
        PNN,
        /// <summary>
        /// SOM neural network.
        /// </summary>
        ///
        SOM
    }
}