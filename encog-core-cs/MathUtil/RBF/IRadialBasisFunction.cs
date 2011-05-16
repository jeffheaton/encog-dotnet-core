namespace Encog.MathUtil.RBF
{
    /// <summary>
    /// A multi-dimension RBF.
    /// </summary>
    ///
    public interface IRadialBasisFunction
    {
        /// <summary>
        /// Set the peak.
        /// </summary>
        ///
        /// <value>The peak.</value>
        double Peak { /// <summary>
            /// Get the center of this RBD.
            /// </summary>
            ///
            /// <returns>The center of the RBF.</returns>
            get;
            /// <summary>
            /// Set the peak.
            /// </summary>
            ///
            /// <param name="peak">The peak.</param>
            set; }


        /// <summary>
        /// Set the width.
        /// </summary>
        ///
        /// <value>The width.</value>
        double Width { /// <returns>The width of the RBF.</returns>
            get;
            /// <summary>
            /// Set the width.
            /// </summary>
            ///
            /// <param name="radius">The width.</param>
            set; }


        /// <value>The dimensions in this RBF.</value>
        int Dimensions { /// <returns>The dimensions in this RBF.</returns>
            get; }


        /// <summary>
        /// Set the centers.
        /// </summary>
        ///
        /// <value>The centers.</value>
        double[] Centers { /// <returns>Get the centers.</returns>
            get;
            /// <summary>
            /// Set the centers.
            /// </summary>
            ///
            /// <param name="center">The centers.</param>
            set; }

        /// <summary>
        /// Calculate the RBF result for the specified value.
        /// </summary>
        ///
        /// <param name="x">The value to be passed into the RBF.</param>
        /// <returns>The RBF value.</returns>
        double Calculate(double[] x);
    }
}