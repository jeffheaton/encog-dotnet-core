// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if logging
using log4net;
#endif

namespace Encog.Util.MathUtil.RBF
{
    /// <summary>
    /// Implements a radial function based on the gaussian function.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
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

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly static ILog LOGGER = LogManager.GetLogger(typeof(GaussianFunction));
#endif

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
