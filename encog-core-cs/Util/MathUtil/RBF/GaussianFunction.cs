using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Encog.Util.MathUtil.RBF
{
    /**
 * Implements a radial function based on the gaussian function.
 * 
 * @author jheaton
 * 
 */
    public class GaussianFunction : IRadialBasisFunction
    {

        /// <summary>
        /// The center of the RBF.
        /// </summary>
        private double center;

        /// <summary>
        /// The peak of the RBF.
        /// </summary>
        private double peak;

        /// <summary>
        /// The width of the RBF.
        /// </summary>
        private double width;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly static ILog LOGGER = LogManager.GetLogger(typeof(GaussianFunction));


        /// <summary>
        /// Construct a Gaussian RBF with the specified center, peak and
        /// width.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="peak">The peak.</param>
        /// <param name="width">The width.</param>
        public GaussianFunction(double center, double peak,
                 double width)
        {
            this.center = center;
            this.peak = peak;
            this.width = width;
        }

        /// <summary>
        /// Calculate the value of the Gaussian function for the specified
        /// value.
        /// </summary>
        /// <param name="x">The value to calculate the Gaussian function for.</param>
        /// <returns>The return value for the Gaussian function.</returns>
        public double Calculate(double x)
        {
            return this.peak
                    * BoundMath.Exp(-BoundMath.Pow(x - this.center, 2)
                            / (2.0 * this.width * this.width));
        }

        /// <summary>
        /// Calculate the value of the derivative of the Gaussian function 
        /// for the specified value.
        /// </summary>
        /// <param name="x">The value to calculate the derivative Gaussian 
        /// function for.</param>
        /// <returns>The return value for the derivative of the Gaussian 
        /// function.</returns>
        public double CalculateDerivative(double x)
        {
            return BoundMath.Exp(-0.5 * this.width * this.width * x * x) * this.peak
                    * this.width * this.width
                    * (this.width * this.width * x * x - 1);
        }

        /// <summary>
        /// The center of the RBF.
        /// </summary>
        public double Center
        {
            get
            {
                return this.center;
            }
        }

        /// <summary>
        /// The peak of the RBF.
        /// </summary>
        public double Peak
        {
            get
            {
                return this.peak;
            }
        }

        /// <summary>
        /// The width of the RBF. 
        /// </summary>
        public double Width
        {
            get
            {
                return this.width;
            }
        }

    }

}
