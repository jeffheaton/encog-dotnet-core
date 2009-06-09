using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.MathUtil.RBF
{
    /// <summary>
    /// Provides a generic interface to a radial basis function (RBF). Encog uses
    /// RBF's for a variety of purposes.
    /// </summary>
    public interface IRadialBasisFunction
    {
        /// <summary>
        /// Calculate the RBF result for the specified value.
        /// </summary>
        /// <param name="x">The value to be passed into the RBF.</param>
        /// <returns>The RBF value.</returns>
        double Calculate(double x);

        /// <summary>
        /// Calculate the derivative of the RBF function.
        /// </summary>
        /// <param name="x">The value to calculate for.</param>
        /// <returns>The calculated value.</returns>
        double CalculateDerivative(double x);

        /// <summary>
        /// The center of the RBF.
        /// </summary>
        double Center
        {
            get;
        }

        /// <summary>
        /// The peak of the RBF.
        /// </summary>
        double Peak
        {
            get;
        }

        /// <summary>
        /// The width of the RBF.
        /// </summary>
        double Width
        {
            get;
        }
    }
}
