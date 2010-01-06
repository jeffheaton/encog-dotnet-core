using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.MathUtil.RBF
{
    /// <summary>
    /// Multi-dimensional gaussian function. Do not use this to implement a 1d
    /// function, simply use GaussianFunction for that.
    /// </summary>
    public class GaussianFunctionMulti : RadialBasisFunctionMulti
    {
        /// <summary>
        /// The center of the RBF.
        /// </summary>
        private double[] center;

        /// <summary>
        /// The peak of the RBF.
        /// </summary>
        private double peak;

        /// <summary>
        /// The width of the RBF.
        /// </summary>
        private double[] width;

        /// <summary>
        /// Construct a multi-dimension Gaussian function with the specified peak,
        /// centers and widths. 
        /// </summary>
        /// <param name="peak">The peak for all dimensions.</param>
        /// <param name="center">The centers for each dimension.</param>
        /// <param name="width">The widths for each dimension.</param>
        public GaussianFunctionMulti(double peak, double[] center,
                 double[] width)
        {
            this.center = center;
            this.peak = peak;
            this.width = width;
        }

        /// <summary>
        /// Construct a Gaussian function with the specified number of dimensions.
        /// The peak, center and widths are all the same.
        /// </summary>
        /// <param name="dimensions">The number of dimensions.</param>
        /// <param name="peak">The peak used for all dimensions.</param>
        /// <param name="center">The center used for all dimensions.</param>
        /// <param name="width">The widths used for all dimensions.</param>
        public GaussianFunctionMulti(int dimensions, double peak,
                 double center, double width)
        {
            this.peak = peak;
            this.center = new double[dimensions];
            this.width = new double[dimensions];
            for (int i = 0; i < dimensions; i++)
            {
                this.center[i] = center;
                this.width[i] = width;
            }
        }

        /// <summary>
        /// Calculate thre result from the function.
        /// </summary>
        /// <param name="x">The parameters for the function, one for each dimension.</param>
        /// <returns>The result of the function.</returns>
        public double Calculate(double[] x)
        {
            double value = 0;

            for (int i = 0; i < this.center.Length; i++)
            {
                value += Math.Pow(x[i] - this.center[i], 2)
                        / (2.0 * this.width[i] * this.width[i]);
            }
            return this.peak * BoundMath.Exp(-value);
        }

        /// <summary>
        /// Get the center for the specified dimension.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        /// <returns>The center.</returns>
        public double GetCenter(int dimension)
        {
            return this.center[dimension];
        }

        /// <summary>
        /// The number of dimensions.
        /// </summary>
        public int Dimensions
        {
            get
            {
                return this.center.Length;
            }
        }

        /// <summary>
        /// The peak.
        /// </summary>
        public double Peak
        {
            get
            {
                return this.peak;
            }
        }

        /// <summary>
        /// Get the width for one dimension.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        /// <returns>The width.</returns>
        public double GetWidth(int dimension)
        {
            return this.width[dimension];
        }

        /// <summary>
        /// Set the width for all dimensions.
        /// </summary>
        /// <param name="w">The width.</param>
        public void SetWidth(double w)
        {
            for (int i = 0; i < this.width.Length; i++)
            {
                this.width[i] = w;
            }
        }
    }
}
