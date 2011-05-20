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
        double Peak { get; set; }


        /// <summary>
        /// Set the width.
        /// </summary>
        double Width { 
            get;
            set; }


        /// <value>The dimensions in this RBF.</value>
        int Dimensions { 
            get; }


        /// <summary>
        /// Set the centers.
        /// </summary>
        double[] Centers { 
            get;
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