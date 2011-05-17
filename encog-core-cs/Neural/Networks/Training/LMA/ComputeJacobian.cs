namespace Encog.Neural.Networks.Training.Lma
{
    /// <summary>
    /// Calculate the Jacobian.
    /// </summary>
    ///
    public interface ComputeJacobian
    {
        /// <value>The Jacobian matrix after it is calculated.</value>
        double[][] Jacobian { get; }


        /// <value>The errors for each row of the matrix.</value>
        double[] RowErrors { get; }

        /// <summary>
        /// Calculate the Jacobian.
        /// </summary>
        ///
        /// <param name="weights">The neural network weights and bias values.</param>
        /// <returns>The sum of squared errors.</returns>
        double Calculate(double[] weights);
    }
}