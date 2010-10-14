// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if logging
using log4net;
using Encog.MathUtil;
using Encog.Engine.Util;
#endif

namespace Encog.MathUtil.RBF
{
    /// <summary>
    /// Implements a radial function based on the inverse multiquadric function.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class InverseMultiquadricFunction : IRadialBasisFunction
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
        private readonly static ILog LOGGER = LogManager.GetLogger(typeof(InverseMultiquadricFunction));
#endif

        /// <summary>
        /// Construct an Inverse Multiquadric RBF with the specified center, peak and
        /// width.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="peak">The peak.</param>
        /// <param name="width">The width.</param>
        public InverseMultiquadricFunction(double center, double peak,
                 double width)
        {
            this.center = center;
            this.peak = peak;
            this.width = width;
        }

        /// <summary>
        /// Calculate the value of the Inverse Multiquadric function for the specified
        /// value.
        /// </summary>
        /// <param name="x">The value to calculate the Inverse Multiquadric function for.</param>
        /// <returns>The return value for the Inverse Multiquadric function.</returns>
        public double Calculate(double x)
        {
            return this.peak
                    / BoundMath.Sqrt(BoundMath.Pow(x - this.center, 2)
                            + (this.width * this.width));
        }

        /// <summary>
        /// Calculate the value of the second derivative of the Inverse Multiquadric function 
        /// for the specified value.
        /// </summary>
        /// <param name="x">The value to calculate the derivative Inverse Multiquadric 
        /// function for.</param>
        /// <returns>The return value for the derivative of the Inverse Multiquadric 
        /// function.</returns>
        public double CalculateFirstDerivative(double x)
        {
            return -1 * this.peak * (x - this.center)
                    / BoundMath.Pow(BoundMath.Pow(x - this.center,2) + (this.width * this.width), 1.5);
        }

        /// <summary>
        /// Maintain implementation of first derivative calculation.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double CalculateDerivative(double x)
        {
            return CalculateFirstDerivative(x);
        }

        /// <summary>
        /// Calculate the value of the second derivative of the Inverse Multiquadric function 
        /// for the specified value.
        /// </summary>
        /// <param name="x">The value to calculate the derivative Inverse Multiquadric 
        /// function for.</param>
        /// <returns>The return value for the derivative of the Inverse Multiquadric 
        /// function.</returns>
        public double CalculateSecondDerivative(double x)
        {
            return this.peak * (this.width * this.width + 2 * BoundMath.Pow(x - this.center, 2))
                / BoundMath.Pow(BoundMath.Pow(x - this.center, 2) + this.width * this.width, 2.5);
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
            set
            {
                this.center = value;
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
            set
            {
                this.width = value;
            }
        }

    }

}

