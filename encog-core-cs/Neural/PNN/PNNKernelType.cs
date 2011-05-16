namespace Encog.Neural.PNN
{
    /// <summary>
    /// Specifies the kernel type for the PNN.
    /// </summary>
    ///
    public enum PNNKernelType
    {
        /// <summary>
        /// A Gaussian curved kernel. The usual choice.
        /// </summary>
        ///
        Gaussian,

        /// <summary>
        /// A steep kernel.
        /// </summary>
        ///
        Reciprocal
    }
}