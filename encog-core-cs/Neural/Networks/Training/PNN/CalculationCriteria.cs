namespace Encog.Neural.Networks.Training.PNN
{
    /// <summary>
    /// Calculate criteria.
    /// </summary>
    ///
    public interface CalculationCriteria
    {
        /// <summary>
        /// Calculate the error with a single sigma.
        /// </summary>
        ///
        /// <param name="sigma">The sigma.</param>
        /// <returns>The error.</returns>
        double CalcErrorWithSingleSigma(double sigma);

        /// <summary>
        /// Calculate the error with multiple sigmas.
        /// </summary>
        ///
        /// <param name="x">The data.</param>
        /// <param name="direc">The first derivative.</param>
        /// <param name="deriv2">The 2nd derivatives.</param>
        /// <param name="b">Calculate the derivative.</param>
        /// <returns>The error.</returns>
        double CalcErrorWithMultipleSigma(double[] x, double[] direc,
                                          double[] deriv2, bool b);
    }
}