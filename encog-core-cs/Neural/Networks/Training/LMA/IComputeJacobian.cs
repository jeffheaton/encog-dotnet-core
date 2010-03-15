using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.LMA
{
    /// <summary>
    /// Calculate the Jacobian.
    /// </summary>
    public interface IComputeJacobian
    {
        /// <summary>
        /// Calculate the Jacobian.
        /// </summary>
        /// <param name="weights">The neural network weights and thresholds.</param>
        /// <returns>The sum of squared errors.</returns>
        double Calculate(double[] weights);

        /// <summary>
        /// The Jacobian matrix after it is calculated.
        /// </summary>
        double[][] Jacobian { get; }

        /// <summary>
        /// The errors for each row of the matrix.
        /// </summary>
        double[] RowErrors { get; }
    }
}
