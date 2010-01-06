using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.MathUtil.RBF
{
    /// <summary>
    /// A multi-dimension RBF.
    /// </summary>
    public interface RadialBasisFunctionMulti
    {
        /// <summary>
        /// Calculate the RBF result for the specified value.
        /// </summary>
        /// <param name="x">The value to be passed into the RBF.</param>
        /// <returns>The RBF value.</returns>
        double Calculate(double[] x);

        /// <summary>
        /// Get the center of this RBD.
        /// </summary>
        /// <param name="dimension">The dimension to get the center for.</param>
        /// <returns>The center of the RBF.</returns>
        double GetCenter(int dimension);

        /// <summary>
        /// The center of the RBF.
        /// </summary>
        double Peak
        {
            get;
        }

        /// <summary>
        /// Get the center of this RBD.
        /// </summary>
        /// <param name="dimension">The dimension to get the center for.</param>
        /// <returns>The center of the RBF.</returns>
        double GetWidth(int dimension);

        /// <summary>
        /// The dimensions in this RBF.
        /// </summary>
        int Dimensions
        {
            get;
        }

        /// <summary>
        /// Set the width.
        /// </summary>
        /// <param name="radius">The width.</param>
        void SetWidth(double radius);
    }
}
