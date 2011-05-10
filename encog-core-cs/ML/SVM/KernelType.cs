namespace Encog.ML.SVM
{
    /// <summary>
    /// The type of SVM kernel in use.
    /// </summary>
    public enum KernelType
    {
        /// <summary>
        /// Linear kernel.
        /// </summary>
        Linear,

        /// <summary>
        /// Poly kernel.
        /// </summary>
        Poly,

        /// <summary>
        /// Radial basis function kernel.
        /// </summary>
        RadialBasisFunction,

        /// <summary>
        /// Sigmoid kernel.
        /// </summary>
        Sigmoid,

        /// <summary>
        /// Precomputed kernel.
        /// </summary>
        Precomputed
    }
}
