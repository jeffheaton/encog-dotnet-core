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

namespace Encog.MathUtil
{
    /// <summary>
    /// C# will sometimes return Math.NaN or Math.Infinity when numbers get to
    /// large or too small. This can have undesirable effects. This class provides
    /// some basic math functions that may be in danger of returning such a value.
    /// This class imposes a very large and small ceiling and floor to keep the
    /// numbers within range.
    /// </summary>
    public sealed class BoundMath
    {

        /// <summary>
        /// Private constructor.
        /// </summary>
        private BoundMath()
        {

        }

        /// <summary>
        /// Calculate the cos.
        /// </summary>
        /// <param name="a">The value passed to the function.</param>
        /// <returns>The result of the function.</returns>
        public static double Cos(double a)
        {
            return BoundNumbers.Bound(Math.Cos(a));
        }

        /// <summary>
        /// Calculate the exp.
        /// </summary>
        /// <param name="a">The value passed to the function.</param>
        /// <returns>The result of the function.</returns>
        public static double Exp(double a)
        {
            return BoundNumbers.Bound(Math.Exp(a));
        }

        /// <summary>
        /// Calculate the log.
        /// </summary>
        /// <param name="a">The value passed to the function.</param>
        /// <returns>The result of the function.</returns>
        public static double Log(double a)
        {
            return BoundNumbers.Bound(Math.Log(a));
        }

        /// <summary>
        /// Calculate the power of a number.
        /// </summary>
        /// <param name="a">The base.</param>
        /// <param name="b">The exponent.</param>
        /// <returns></returns>
        public static double Pow(double a, double b)
        {
            return BoundNumbers.Bound(Math.Pow(a, b));
        }

        /// <summary>
        /// Calculate the sin.
        /// </summary>
        /// <param name="a">The value passed to the function.</param>
        /// <returns>The result of the function.</returns>
        public static double Sin(double a)
        {
            return BoundNumbers.Bound(Math.Sin(a));
        }

        /// <summary>
        /// Calculate the square root.
        /// </summary>
        /// <param name="a">The value passed to the function.</param>
        /// <returns>The result of the function.</returns>
        public static double Sqrt(double a)
        {
            return Math.Sqrt(a);
        }
    }

}
